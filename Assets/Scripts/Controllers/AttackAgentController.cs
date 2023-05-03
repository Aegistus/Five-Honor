using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test controller for attack tests
/// </summary>
public class AttackAgentController : AgentController
{
    GuardDirection currentDirection = GuardDirection.Left;

    private void Start()
    {
        StartCoroutine(Attack());
        StartCoroutine(ChangeAttackDirection());
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            LightAttack = true;
            yield return null;
            LightAttack = false;
        }
    }

    IEnumerator ChangeAttackDirection()
    {
        while (true)
        {
            yield return null;
            yield return new WaitForSeconds(2f);
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    currentDirection = GuardDirection.Left;
                    break;
                case 1:
                    currentDirection = GuardDirection.Right;
                    break;
                case 2:
                    currentDirection = GuardDirection.Top;
                    break;
            }
        }
    }

    public override GuardDirection GetGuardDirection()
    {
        return currentDirection;
    }

    public override void FindNewTarget()
    {
        
    }
}
