using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] Transform staminaBar;

    AgentStamina playerStamina;

    private void Start()
    {
        playerStamina = FindObjectOfType<PlayerController>().GetComponent<AgentStamina>();
        playerStamina.OnStaminaChange += UpdateHealthBar;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float percentHealth = playerStamina.CurrentStamina / playerStamina.MaxStamina;
        staminaBar.localScale = new Vector2(percentHealth, 1);
    }

}
