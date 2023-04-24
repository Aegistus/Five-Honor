using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentHealth : MonoBehaviour
{
    public event Action OnDamageTaken;
    public event Action OnDamageBlocked;

    float currentHealth = 100f;

    AgentMovement movement;

    private void Start()
    {
        movement = GetComponent<AgentMovement>();
    }

    /// <summary>
    /// Attempt to do damage to agent. If the attack direction != the agent's guard direction, the attack will succeed.
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
        currentHealth -= damage;
        OnDamageTaken?.Invoke();
    }
}
