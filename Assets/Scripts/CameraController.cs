using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject passiveCam;
    [SerializeField] GameObject combatCam;
    [SerializeField] Transform playerFollowTarget;
    [SerializeField] Transform enemyTarget;
    [SerializeField] float mouseSensitivity = 1000f;

    PlayerMovement movement;

    private void Awake()
    {
        movement = FindObjectOfType<PlayerMovement>();
        movement.OnStanceChange += ChangeStanceCamera;
    }

    private void ChangeStanceCamera(StanceType stance)
    {
        if (stance == StanceType.Passive)
        {
            passiveCam.SetActive(true);
            combatCam.SetActive(false);
        }
        else if (stance == StanceType.Combat)
        {
            passiveCam.SetActive(false);
            combatCam.SetActive(true);
        }
    }

    private void Update()
    {
        if (movement.CurrentStance == StanceType.Combat)
        {
            CombatCameraRotation();
        }
        else if (movement.CurrentStance == StanceType.Passive)
        {
            PassiveCameraRotation();
        }
    }


    void PassiveCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        playerFollowTarget.Rotate(-mouseY * mouseSensitivity * Time.deltaTime, mouseX * mouseSensitivity * Time.deltaTime, 0);
        playerFollowTarget.eulerAngles = new Vector3(playerFollowTarget.eulerAngles.x, playerFollowTarget.eulerAngles.y, 0);
    }

    void CombatCameraRotation()
    {
        playerFollowTarget.LookAt(enemyTarget.position);
    }

}
