using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MovementType
{
    Standing, Running, Sprinting, Strafing
}
public enum StanceType
{
    Passive, Combat
}

public class PlayerMovement : MonoBehaviour
{
    public MovementType CurrentStateType { get; private set; }
    public event Action<MovementType> OnMovementStateChange;
    public event Action<StanceType> OnStanceChange;
    public float CurrentMoveSpeed { get; private set; }
    public Vector3 CurrentMoveVector => movementDirection.InverseTransformVector(movementVector);
    public float MaxRunSpeed => runSpeed;
    public StanceType CurrentStance => currentStance;

    [SerializeField] Transform playerModel;
    [SerializeField] Transform followTarget;
    [SerializeField] Transform movementDirection;
    [SerializeField] Transform target;
    [SerializeField] float modelTurnSpeed = 60f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float runAcceleration = .5f;
    [SerializeField] float strafeSpeed = 4f;
    [SerializeField] float strafeAcceleration = .5f;

    Vector3 movementVector;
    Quaternion targetRotation, currentRotation;

    Dictionary<MovementType, MovementState> movementStates;
    MovementType defaultMovementState;

    MovementState currentState;
    StanceType currentStance;

    private void Awake()
    {
        movementStates = new Dictionary<MovementType, MovementState>()
        {
            { MovementType.Standing, new StandingState(this) },
            { MovementType.Running, new RunningState(this) },
            { MovementType.Strafing, new Strafing(this) },
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
        // change stance
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (currentStance == StanceType.Passive)
            {
                ChangeStance(StanceType.Combat);
            }
            else
            {
                ChangeStance(StanceType.Passive);
            }
        }
        // set to default state if null
        if (currentState == null)
        {
            currentState = movementStates[defaultMovementState];
            currentState.BeforeExecution();
            CurrentStateType = defaultMovementState;
            OnMovementStateChange?.Invoke(CurrentStateType);
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
            OnMovementStateChange?.Invoke(CurrentStateType);
        }
        currentState.DuringExecution();
    }

    private void FixedUpdate()
    {
        currentState?.DuringPhysicsUpdate();
    }

    void MoveLaterally(float moveSpeed)
    {
        // lateral movement
        movementDirection.eulerAngles = new Vector3(0, followTarget.eulerAngles.y, 0);
        movementVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movementVector += movementDirection.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector -= movementDirection.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector -= movementDirection.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector += movementDirection.right;
        }
        movementVector.Normalize();
        transform.Translate(moveSpeed * Time.deltaTime * movementVector, Space.World);
        CurrentMoveSpeed = moveSpeed;
    }
    
    void CombatRotatePlayerModel()
    {
        currentRotation = playerModel.rotation;
        playerModel.LookAt(target);
        targetRotation.eulerAngles = new Vector3(0, playerModel.eulerAngles.y, 0);
        playerModel.rotation = currentRotation;
        playerModel.rotation = Quaternion.Lerp(currentRotation, targetRotation, modelTurnSpeed * Time.deltaTime);
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

    void ChangeStance(StanceType newStance)
    {
        currentStance = newStance;
        OnStanceChange?.Invoke(newStance);
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
            print("Standing");
        }

        public override MovementType? CheckTransitions()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                if (movement.CurrentStance == StanceType.Combat)
                {
                    return MovementType.Strafing;
                }
                else
                {
                    return MovementType.Running;
                }
            }
            return null;
        }

        public override void DuringExecution()
        {
            if (movement.currentStance == StanceType.Combat)
            {
                movement.CombatRotatePlayerModel();
            }
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class RunningState : MovementState
    {
        float currentRunSpeed = 0f;

        public RunningState(PlayerMovement movement) : base(movement) { }

        public override void AfterExecution()
        {
            
        }

        public override void BeforeExecution()
        {
            print("Running");
            currentRunSpeed = 0;
        }

        public override MovementType? CheckTransitions()
        {
            if (movement.CurrentStance == StanceType.Combat)
            {
                return MovementType.Strafing;
            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentRunSpeed = Mathf.Lerp(movement.CurrentMoveSpeed, movement.runSpeed, movement.runAcceleration * Time.deltaTime);
            movement.MoveLaterally(currentRunSpeed);
            movement.RotatePlayerModel();
        }

        public override void DuringPhysicsUpdate()
        {
            
        }
    }

    class Strafing : MovementState
    {
        float currentStrafeSpeed = 0f;
        
        public Strafing(PlayerMovement movement) : base(movement)
        {
        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Strafing");
            currentStrafeSpeed = 0f;
        }

        public override MovementType? CheckTransitions()
        {
            if (movement.currentStance != StanceType.Combat)
            {
                return MovementType.Running;
            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentStrafeSpeed = Mathf.Lerp(movement.CurrentMoveSpeed, movement.strafeSpeed, movement.runAcceleration * Time.deltaTime);
            movement.MoveLaterally(currentStrafeSpeed);
            movement.CombatRotatePlayerModel();
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    #endregion
}
