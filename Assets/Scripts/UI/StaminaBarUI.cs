using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] Transform staminaBar;

    AgentStamina playerStamina;
    Transform mainCam;

    private void Start()
    {
        playerStamina = FindObjectOfType<PlayerController>().GetComponent<AgentStamina>();
        mainCam = Camera.main.transform;
    }

    private void Update()
    {
        float percentStamina = playerStamina.CurrentStamina / playerStamina.MaxStamina;
        staminaBar.localScale = new Vector2(percentStamina, 1);
        transform.LookAt(mainCam);
    }

}
