using UnityEngine;
using System.Collections;
using System;

public class CharacterData : AriaBehaviour, IHealth
{
    public int currentHealth = 100;
    public int maxHealth = 100;
    private bool healthChanged = false;

    private int strength = 12; // Physical strength/speed
    private int agility = 12; // Physical accuracy
    private int discipline = 12; // Mental strength/speed
    private int intelligence = 12; // Mental accuracy
    private int spirit = 12; // Magical strength/speed
    private int wisdom = 12; // Magical accuracy

    float IHealth.current
    {
        get
        {
            return currentHealth;
        }

        set
        {
            if (value == 0)
            {
                (this as IHealth).Die();
            }
            else if (currentHealth != value)
            {
                currentHealth = (int)value;
                healthChanged = true;
            }
        }
    }

    float IHealth.max
    {
        get
        {
            return maxHealth;
        }

        set
        {
            if(maxHealth != value)
            {
                maxHealth = (int)value;
                healthChanged = true;
            }
        }
    }

    bool IHealth.changed
    {
        get
        {
            return healthChanged;
        }
        set
        {
            healthChanged = value;
        }
    }

    void IHealth.Die()
    {
        HealthBar healthBar = this.GetComponent<HealthBar>();
        if(healthBar)
        {
            GameObject.Destroy(healthBar.healthBarInstance.gameObject);
        }
        GameObject.Destroy(this.gameObject);
    }
}
