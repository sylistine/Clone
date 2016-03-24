using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    public float interestDist;
    public float moveSpeed;

    private CharacterData charData;

    private enum State
    {
        Idling,
        Attacking
    }

    private enum StateTransition
    {
        Idle2Attack,
        Attack2Idle
    }
    
    private State state;

    private Transform _player;
    private Transform player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.Find("Player").transform;
            }
            return _player;
        }
    }
    private SphereCollider _sightTrigger;
    private SphereCollider sightTrigger
    {
        get
        {
            if (_sightTrigger == null)
            {
                _sightTrigger = this.GetComponent<SphereCollider>();
            }
            return _sightTrigger;
        }
    }

	void Start ()
    {
        charData = this.GetComponent<CharacterData>();
	}

    void Update()
    {
        switch (state)
        {
            case State.Idling:
                IdleUpdate();
                break;
            case State.Attacking:
                AttackUpdate();
                break;
            default:
                throw new System.NullReferenceException("State machine error. State not found. Current state is: " + state.ToString());
        }
    }

    void IdleUpdate()
    {
        // Set idle animation
    }

    void AttackUpdate()
    {
        this.transform.rotation = Quaternion.LookRotation(player.transform.position - this.transform.position, Vector3.up);
        float playerDist = Vector3.Distance(this.transform.position, player.transform.position);
        Debug.Log(playerDist);
        if (playerDist < charData.attackRange)
        {
            if (charData.attackCooldown == 0)
            {
                var playerHealth = player.GetComponent<IHealth>();
                playerHealth.current -= charData.attackDamage;
                charData.attackCooldown = 1 / charData.attacksPerSecond;
            }
        }
        else
        {
            this.transform.position += this.transform.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
        }
        if (playerDist > interestDist)
            state = State.Idling;
    }

    void OnTriggerEnter(Collider other)
    {
        if(state == State.Idling)
        {
            if (other.transform == player)
            {
                state = State.Attacking;
            }
        }
    }
}
