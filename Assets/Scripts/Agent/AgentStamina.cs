using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStamina : MonoBehaviour
{
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float staminaRegainRate = 50f;

    public event Action OnStaminaChange;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;

    private void Start()
    {
        CurrentStamina = maxStamina;
    }

    public bool TrySpendStamina(float stamina)
    {
        if (CurrentStamina >= stamina)
        {
            SpendStamina(stamina);
            return true;
        }
        else
        {
            return false;
        }
    }

    void SpendStamina(float stamina)
    {
        CurrentStamina -= stamina;
        if (CurrentStamina < 0)
        {
            CurrentStamina = 0f;
        }
        OnStaminaChange?.Invoke();
    }

    private void Update()
    {
        if (CurrentStamina < MaxStamina)
        {
            CurrentStamina += staminaRegainRate * Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        }
    }


}
