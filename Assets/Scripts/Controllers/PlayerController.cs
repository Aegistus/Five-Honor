using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AgentController
{
    [SerializeField] float guardChangeSensitivity = .5f;

    Vector2 lastMousePosition;

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
        GuardDirection newDirection = GuardDirection.None;
        Vector2 currentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 mousePosDelta = lastMousePosition - currentMousePosition;
        if (mousePosDelta.x < -guardChangeSensitivity && mousePosDelta.y < guardChangeSensitivity)
        {
            newDirection = GuardDirection.Left;
        }
        else if (mousePosDelta.x > guardChangeSensitivity && mousePosDelta.y < guardChangeSensitivity)
        {
            newDirection = GuardDirection.Right;
        }
        else if (mousePosDelta.y > guardChangeSensitivity)
        {
            newDirection = GuardDirection.Top;
        }
        lastMousePosition = currentMousePosition;
        return newDirection;
    }
}
