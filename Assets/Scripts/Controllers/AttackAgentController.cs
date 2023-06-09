using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test controller for attack tests
/// </summary>
public class AttackAgentController : AgentController
{
    [SerializeField] Transform lookDirection;

    GuardDirection currentDirection = GuardDirection.Left;
    float minDistanceFromPlayer = 5f;

    bool inAttackRange = false;

    private void Start()
    {
        Target = FindObjectOfType<PlayerController>().transform;

        StartCoroutine(Attack());
        StartCoroutine(ChangeAttackDirection());
        StartCoroutine(SwitchToGuardStance());
    }

    private void Update()
    {
        if (Target != null)
        {
            lookDirection.LookAt(Target.position);
        }
        if (Vector3.Distance(transform.position, Target.position) > minDistanceFromPlayer)
        {
            Forwards = true;
            inAttackRange = false;
        }
        else
        {
            Forwards = false;
            inAttackRange = true;
        }
    }

    IEnumerator SwitchToGuardStance()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<AgentMovement>().ChangeStance(StanceType.Combat);
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (inAttackRange)
            {
                LightAttack = true;
                yield return null;
                LightAttack = false;
            }
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
