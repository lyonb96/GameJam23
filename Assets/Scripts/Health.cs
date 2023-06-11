using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth;

    public bool KillOnDeath;

    public int CurrentHealth { get; private set; }

    public float CurrentHealthPercent => (float)CurrentHealth / MaxHealth;

    public UnityEvent OnDeath;

    public UnityEvent OnDamage;

    private bool Immune;

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
        if (CurrentHealth <= 0 || Immune)
        {
            return;
        }
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

    public void SetImmune(bool immune)
    {
        Immune = immune;
    }
}
