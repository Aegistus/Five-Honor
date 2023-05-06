using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test controller for attack tests
/// </summary>
public class AttackAgentController : AgentController
{
    [SerializeField] Transform followTarget;

    GuardDirection currentDirection = GuardDirection.Left;
    float minDistanceFromPlayer = 5f;

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
            followTarget.LookAt(Target.position);
        }
        if (Vector3.Distance(transform.position, Target.position) > minDistanceFromPlayer)
        {
            Forwards = true;
        }
        else
        {
            Forwards = false;
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
