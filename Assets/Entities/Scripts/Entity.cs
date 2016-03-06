using UnityEngine;
using System.Collections;

public class Entity : AriaBehaviour
{
    public Transform healthBarCanvas;
    public RectTransform healthBarGUI;
    public float healthBarHeight;
    private RectTransform healthBar;
    private Vector3 healthBarPos;

    [SerializeField]
    protected float _health;
    [SerializeField]
    protected float maxHealth = 1000;
    public float health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    void Start()
    {
        healthBar = GameObject.Instantiate(healthBarGUI);
        healthBar.SetParent(healthBarCanvas, false);


        healthBarPos = this.transform.position;
        healthBarPos.y += healthBarHeight;
        healthBar.anchoredPosition = Camera.main.WorldToScreenPoint(healthBarPos);
    }

    void Update()
    {
        healthBarPos = this.transform.position;
        healthBarPos.y += healthBarHeight;
        healthBar.anchoredPosition = Camera.main.WorldToScreenPoint(healthBarPos);
    }
}
