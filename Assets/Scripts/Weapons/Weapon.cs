using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    public event Action OnAttackBlocked;

    [SerializeField] float damage = 10f;

    bool inDamageState = false;
    GuardDirection attackDirection = GuardDirection.None;

    public void EnterDamageState(float attackDuration, GuardDirection direction)
    {
        attackDirection = direction;
        inDamageState = true;
        StartCoroutine(AttackRoutine(attackDuration));
    }

    IEnumerator AttackRoutine(float attackDuration)
    {
        yield return new WaitForSeconds(attackDuration);
        inDamageState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inDamageState)
        {
            AgentHealth health = GetComponentInParent<AgentHealth>();
            if (health != null)
            {
                bool attackSucceeded = health.AttemptDamage(damage, attackDirection);
                if (!attackSucceeded)
                {
                    OnAttackBlocked?.Invoke();
                }
            }
        }
    }
}
