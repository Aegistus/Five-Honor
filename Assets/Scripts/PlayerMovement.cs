using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform playerModel;
    [SerializeField] Transform followTarget;
    [SerializeField] Transform movementTransform;
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField] float modelTurnSpeed = 60f;
    [SerializeField] float moveSpeed = 5f;

    Vector3 movementVector;
    Quaternion targetRotation, currentRotation;

    private void Update()
    {
        // lateral movement
        movementTransform.eulerAngles = new Vector3(0, followTarget.eulerAngles.y, 0);
        movementVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movementVector += movementTransform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector -= movementTransform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector -= movementTransform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector += movementTransform.right;
        }
        movementVector.Normalize();
        transform.Translate(moveSpeed * Time.deltaTime * movementVector, Space.World);

        // mouse rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        followTarget.Rotate(-mouseY * mouseSensitivity * Time.deltaTime, mouseX * mouseSensitivity * Time.deltaTime, 0);
        followTarget.eulerAngles = new Vector3(followTarget.eulerAngles.x, followTarget.eulerAngles.y, 0);

        // model rotation
        currentRotation = playerModel.rotation;
        playerModel.LookAt(playerModel.position + movementVector);
        targetRotation.eulerAngles = new Vector3(0, playerModel.eulerAngles.y, 0);
        playerModel.rotation = currentRotation;
        playerModel.rotation = Quaternion.Lerp(currentRotation, targetRotation, modelTurnSpeed * Time.deltaTime);
    }
}
