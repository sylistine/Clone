using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterData))]
public class SlimeAI : StateMachine
{
    public float attackModeMinDistanceThreshold;
    public float attackModeMaxDistanceThreshold;

    private Transform target;
    private float targetDistance;

    private CharacterData characterData;
    private Animator animator;
    
    private State attack;
    private State idle;
    private StateTransition toIdle;
    private StateTransition toAttack;

    void Start()
    {
        target = GameController.instance.player;

        characterData = this.GetComponent<CharacterData>();
        animator = this.GetComponent<Animator>();
        
        attack = new Attack(this);
        idle = new Idle(this);
        toIdle = new StateTransition(this, idle);
        toAttack= new StateTransition(this, attack);

        currentState = idle;
    }

    void Update()
    {
        targetDistance = Vector3.Distance(this.transform.position, target.position);
        currentState.StateUpdate();
    }

    #region States
    private class Attack : State
    {
        private readonly SlimeAI stateMachine;

        public Attack (SlimeAI stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public override void StateEnter()
        {
            Debug.Log("[SlimeAI] Slime has entered attack mode!");
            base.StateEnter();
        }

        public override void StateUpdate()
        {
            Debug.Log("Updating attack state");
            base.StateUpdate();
            if(stateMachine.targetDistance > stateMachine.attackModeMaxDistanceThreshold)
            {
                stateMachine.toIdle.Execute();
            }
            else
            {
                if(stateMachine.characterData.attackRange < stateMachine.targetDistance)
                {
                    // Move toward player
                    Vector3 moveDir = (stateMachine.target.position - stateMachine.transform.position).normalized;
                    stateMachine.transform.LookAt(stateMachine.transform.position + moveDir);
                    stateMachine.transform.position += stateMachine.transform.TransformDirection(Vector3.forward) * Time.deltaTime;
                }
                else
                {
                    // Attack
                }
            }
        }
    }

    private class Idle : State
    {
        private readonly SlimeAI stateMachine;
        private Vector3 idleOrigin;

        public Idle (SlimeAI stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public override void StateEnter()
        {
            Debug.Log("[SlimeAI] Slime has entered idle mode.");
            base.StateEnter();
            idleOrigin = stateMachine.transform.position;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            if(stateMachine.targetDistance < stateMachine.attackModeMinDistanceThreshold)
            {
                stateMachine.toAttack.Execute();
            }
            else
            {
                // Move semi-randomly around idleOrigin.
            }
        }
    }
    #endregion
}
