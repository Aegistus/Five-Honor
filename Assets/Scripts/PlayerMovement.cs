using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Standing, Running, Sprinting
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform playerModel;
    [SerializeField] Transform followTarget;
    [SerializeField] Transform movementTransform;
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField] float modelTurnSpeed = 60f;
    [SerializeField] float runSpeed = 5f;

    Vector3 movementVector;
    Quaternion targetRotation, currentRotation;

    Dictionary<MovementType, MovementState> movementStates;
    MovementType defaultMovementState;

    public MovementType CurrentStateType { get; private set; }
    MovementState currentState;

    private void Awake()
    {
        movementStates = new Dictionary<MovementType, MovementState>()
        {
            { MovementType.Standing, new StandingState(this) },
        };
        defaultMovementState = MovementType.Standing;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // set to default state if null
        if (currentState == null)
        {
            currentState = movementStates[defaultMovementState];
            currentState.BeforeExecution();
            CurrentStateType = defaultMovementState;
        }
        // check for state transitions
        MovementType? returnedState = currentState.CheckTransitions();
        if (returnedState != null)
        {
            MovementType newState = (MovementType) returnedState;
            currentState.AfterExecution();
            currentState = movementStates[newState];
            currentState.BeforeExecution();
            CurrentStateType = newState;
        }
    }

    void MoveLaterally(float moveSpeed)
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
    }

    void MouseRotation()
    {
        // mouse rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        followTarget.Rotate(-mouseY * mouseSensitivity * Time.deltaTime, mouseX * mouseSensitivity * Time.deltaTime, 0);
        followTarget.eulerAngles = new Vector3(followTarget.eulerAngles.x, followTarget.eulerAngles.y, 0);
    }

    void RotatePlayerModel()
    {
        // model rotation
        currentRotation = playerModel.rotation;
        playerModel.LookAt(playerModel.position + movementVector);
        targetRotation.eulerAngles = new Vector3(0, playerModel.eulerAngles.y, 0);
        playerModel.rotation = currentRotation;
        playerModel.rotation = Quaternion.Lerp(currentRotation, targetRotation, modelTurnSpeed * Time.deltaTime);
    }

    #region Movement States
    abstract class MovementState
    {
        protected PlayerMovement movement;

        public MovementState(PlayerMovement movement)
        {
            this.movement = movement;
        }

        public abstract void BeforeExecution();
        public abstract void DuringExecution();
        public abstract void AfterExecution();
        public abstract void DuringPhysicsUpdate();
        public abstract MovementType? CheckTransitions();
    }

    class StandingState : MovementState
    {
        public StandingState(PlayerMovement movement) : base(movement) { }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Standing State");
        }

        public override MovementType? CheckTransitions()
        {
            return null;
        }

        public override void DuringExecution()
        {

        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class RunningState : MovementState
    {
        public RunningState(PlayerMovement movement) : base(movement) { }

        public override void AfterExecution()
        {
            
        }

        public override void BeforeExecution()
        {
            print("Running State");
        }

        public override MovementType? CheckTransitions()
        {
            return null;
        }

        public override void DuringExecution()
        {
            movement.MoveLaterally(movement.runSpeed);
            movement.MouseRotation();
            movement.RotatePlayerModel();
        }

        public override void DuringPhysicsUpdate()
        {
            
        }
    }

    #endregion
}
