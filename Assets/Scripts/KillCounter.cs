using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KillCounter : MonoBehaviour
{
    private Text textComponent;
    private int _killCount;
    public int killCount
    {
        get
        {
            return _killCount;
        }
        set
        {
            _killCount = value;
            textComponent.text = value.ToString();
        }
    }

    void Start()
    {
        textComponent = this.GetComponent<Text>();
    }
}
