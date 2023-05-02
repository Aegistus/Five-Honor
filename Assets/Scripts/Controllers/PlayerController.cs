using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AgentController
{
    [SerializeField] float guardChangeTolerance = 1f;

    GuardDirection lastGuardDirection;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Forwards = Input.GetKey(KeyCode.W);
        Backwards = Input.GetKey(KeyCode.S);
        Left = Input.GetKey(KeyCode.A);
        Right = Input.GetKey(KeyCode.D);
        Dodge = Input.GetKeyDown(KeyCode.Space);
        Sprint = Input.GetKey(KeyCode.LeftShift);
        StanceChange = Input.GetKeyDown(KeyCode.LeftControl);
        LightAttack = Input.GetMouseButton(0);
    }

    public override GuardDirection GetGuardDirection()
    {
        GuardDirection newDirection = lastGuardDirection;
        Vector2 currentMouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (currentMouseMovement.x < -guardChangeTolerance && currentMouseMovement.y < guardChangeTolerance)
        {
            newDirection = GuardDirection.Left;
        }
        else if (currentMouseMovement.x > guardChangeTolerance && currentMouseMovement.y < guardChangeTolerance)
        {
            newDirection = GuardDirection.Right;
        }
        else if (currentMouseMovement.y > guardChangeTolerance)
        {
            newDirection = GuardDirection.Top;
        }
        return newDirection;
    }
}
