using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth;

    public bool KillOnDeath;

    public int CurrentHealth { get; private set; }

    public UnityEvent OnDeath;

    public UnityEvent OnDamage;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void Awake()
    {
        OnDamage ??= new();
        OnDeath ??= new();
    }

    public void Damage(int damage)
    {
        CurrentHealth -= damage;
        OnDamage.Invoke();
        if (CurrentHealth <= 0)
        {
            OnDeath.Invoke();
            if (KillOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Heal(int health, bool allowOverheal = false)
    {
        if (!allowOverheal)
        {
            CurrentHealth = Math.Max(CurrentHealth + health, MaxHealth);
        }
        else
        {
            CurrentHealth += health;
        }
    }
}
