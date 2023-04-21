using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBarsUI : MonoBehaviour
{
    [SerializeField] GameObject upper;
    [SerializeField] GameObject lower;

    PlayerMovement movement;

    private void Start()
    {
        upper.SetActive(false);
        lower.SetActive(false);
        movement = FindObjectOfType<PlayerMovement>();
        movement.OnStanceChange += UpdateBlackBars;
    }

    private void UpdateBlackBars(StanceType stance)
    {
        if (stance == StanceType.Combat)
        {
            upper.SetActive(true);
            lower.SetActive(true);
        }
        else
        {
            upper.SetActive(false);
            lower.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        movement.OnStanceChange -= UpdateBlackBars;
    }

}
