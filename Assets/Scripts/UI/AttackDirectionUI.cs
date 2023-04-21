using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDirectionUI : MonoBehaviour
{
    [SerializeField] GameObject uiElement;
    [SerializeField] GameObject top;
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;

    PlayerMovement movement;

    private void Awake()
    {
        movement = FindObjectOfType<PlayerMovement>();
        movement.OnStanceChange += StanceChange;
        movement.OnGuardDirectionChange += ChangeGuardDirection;
        uiElement.SetActive(false);
    }

    void StanceChange(StanceType stance)
    {
        if (stance == StanceType.Combat)
        {
            uiElement.SetActive(true);
        }
        else if (stance == StanceType.Passive)
        {
            uiElement.SetActive(false);
        }
    }

    void ChangeGuardDirection(GuardDirection direction)
    {
        if (movement.CurrentStance != StanceType.Combat)
        {
            return;
        }
        top.SetActive(false);
        left.SetActive(false);
        right.SetActive(false);
        if (direction == GuardDirection.Top)
        {
            top.SetActive(true);
        }
        else if (direction == GuardDirection.Left)
        {
            left.SetActive(true);
        }
        else if (direction == GuardDirection.Right)
        {
            right.SetActive(true);
        }
    }


}
