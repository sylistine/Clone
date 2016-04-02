using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : Singleton<GameController>
{
    protected GameController() { }

    public RectTransform healthBarCanvas;
    public Transform levelParent;
    public Transform entityParent;
    public Transform playerPrefab;

    private Transform _player;
    public Transform player {
        get
        {
            return _player;
        }
        set
        {
            _player = value;
        }
    }

    void Start()
    {
        player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as Transform;
        player.parent = entityParent;
        player.GetComponent<HealthBar>().healthBarCanvas = this.healthBarCanvas;
    }
}
