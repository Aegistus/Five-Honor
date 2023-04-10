using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;


    private void Update()
    {
        Vector3 movementVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movementVector += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector += transform.right;
        }
        movementVector.Normalize();
        transform.Translate(movementVector * moveSpeed * Time.deltaTime);
    }
}
