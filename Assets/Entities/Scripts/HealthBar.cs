using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(IHealth))]
public class HealthBar : AriaBehaviour
{
    public Transform healthBarCanvas;
    public RectTransform healthBarPrefab;
    public float healthBarHeight;
    private RectTransform _healthBarInstance;
    private Slider healthBarSlider;
    private Vector3 healthBarPos;

    private IHealth hpSource;

    public RectTransform healthBarInstance
    {
        get
        {
            return _healthBarInstance;
        }
    }


    void Start()
    {
        hpSource = this.GetComponent<IHealth>();

        _healthBarInstance = GameObject.Instantiate(healthBarPrefab);
        _healthBarInstance.SetParent(healthBarCanvas, false);
        healthBarSlider = _healthBarInstance.GetComponentInChildren<Slider>();

        healthBarPos = this.transform.position;
        healthBarPos.y += healthBarHeight;
        _healthBarInstance.anchoredPosition = Camera.main.WorldToScreenPoint(healthBarPos);
    }

    void Update()
    {
        if(hpSource.changed)
        {
            healthBarSlider.value = hpSource.current / hpSource.max;
            hpSource.changed = false;
        }
        healthBarPos = this.transform.position;
        healthBarPos.y += healthBarHeight;
        _healthBarInstance.anchoredPosition = Camera.main.WorldToScreenPoint(healthBarPos);
    }
}
