using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStamina : MonoBehaviour
{
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float staminaRegainRate = 50f;
    [SerializeField] float staminaRegenDelay = 2f;

    public event Action OnStaminaChange;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;

    float currentDelay = 0f;

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
        currentDelay = staminaRegenDelay;
        OnStaminaChange?.Invoke();
    }

    private void Update()
    {
        if (CurrentStamina < MaxStamina)
        {
            if (currentDelay <= 0)
            {
                CurrentStamina += staminaRegainRate * Time.deltaTime;
                CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
            }
            else
            {
                currentDelay -= Time.deltaTime;
            }

        }
    }


}
