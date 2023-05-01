using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform passiveLookTarget;
    [SerializeField] Transform combatLookTarget;
    [SerializeField] Vector3 passiveOffset;
    [SerializeField] Vector3 combatOffset;
    [SerializeField] float mouseSensitivity = 1000f;
    [SerializeField] float stanceChangeSpeed = 5f;

    AgentMovement movement;
    Vector3 targetCameraOffset;

    private void Awake()
    {
        movement = FindObjectOfType<PlayerController>().GetComponent<AgentMovement>();
        movement.OnStanceChange += ChangeStanceCamera;
        ChangeStanceCamera(StanceType.Passive);
    }

    private void ChangeStanceCamera(StanceType stance)
    {
        if (stance == StanceType.Passive)
        {
            targetCameraOffset = passiveOffset;
        }
        else if (stance == StanceType.Combat)
        {
            targetCameraOffset = combatOffset;
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
        // move cam to current offset
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetCameraOffset, stanceChangeSpeed * Time.deltaTime);
    }


    void PassiveCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        cameraHolder.Rotate(-mouseY * mouseSensitivity * Time.deltaTime, mouseX * mouseSensitivity * Time.deltaTime, 0);
        cameraHolder.eulerAngles = new Vector3(cameraHolder.eulerAngles.x, cameraHolder.eulerAngles.y, 0);
    }

    void CombatCameraRotation()
    {
        cameraHolder.LookAt(combatLookTarget.position);
    }

}
