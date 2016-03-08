using UnityEngine;
using System.Collections;

public interface IHealth
{
    float current
    {
        get;
        set;
    }
    float max
    {
        get;
        set;
    }
    bool changed
    {
        get;
        set;
    }

    void Die();
}
