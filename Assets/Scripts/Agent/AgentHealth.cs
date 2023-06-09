using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentHealth : MonoBehaviour
{
    public event Action OnDamageTaken;
    public event Action OnDamageBlocked;
    public event Action OnAgentDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;


    bool isDead = false;

    float maxHealth = 100f;
    float currentHealth = 100f;

    AgentMovement movement;
    int hitSoundID;

    private void Start()
    {
        movement = GetComponent<AgentMovement>();
        hitSoundID = SoundManager.Instance.GetSoundID("Agent_Hit");
    }

    /// <summary>
    /// Attempt to do damage to agent. If the attack direction is not countered by the agent's guard direction, the attack will succeed.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="direction"></param>
    /// <returns>True if the attack succeeded, false otherwise. </returns>
    public bool AttemptDamage(float damage, GuardDirection direction)
    {
        if (movement.CurrentGuardDirection.Counters(direction) && movement.CurrentStance == StanceType.Combat &&
            movement.CurrentStateType != MovementType.Attacking && movement.CurrentStateType != MovementType.Dodging)
        {
            print("Damage blocked");
            OnDamageBlocked?.Invoke();
            return false;
        }
        else
        {
            print("Damage dealt: " + damage);
            Damage(damage);
            return true;
        }
    }

    public void Damage(float damage)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= damage;
        SoundManager.Instance.PlaySoundAtPosition(hitSoundID, transform.position);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }
        OnDamageTaken?.Invoke();
    }

    void Die()
    {
        isDead = true;
        OnDamageTaken?.Invoke();
        OnAgentDeath?.Invoke();
    }
}
