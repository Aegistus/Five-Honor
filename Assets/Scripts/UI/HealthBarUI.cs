using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Transform healthBarTransform;
    [SerializeField] Gradient healthGradient;

    AgentHealth agentHealth;
    RawImage barImage;


    private void Start()
    {
        agentHealth = GetComponentInParent<AgentHealth>();
        agentHealth.OnDamageTaken += UpdateHealthBar;
        barImage = healthBarTransform.GetComponent<RawImage>();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float percentHealth = agentHealth.CurrentHealth / agentHealth.MaxHealth;
        healthBarTransform.localScale = new Vector2(percentHealth, 1);
        barImage.color = healthGradient.Evaluate(1 - percentHealth);
    }

}
