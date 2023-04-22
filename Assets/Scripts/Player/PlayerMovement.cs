using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MovementType
{
    Standing, Running, Sprinting, Strafing, Attacking, Dodging
}
public enum StanceType
{
    Passive, Combat
}

public enum GuardDirection
{
    None, Top, Left, Right
}

public class PlayerMovement : MonoBehaviour
{
    public MovementType CurrentStateType { get; private set; }
    public event Action<MovementType> OnMovementStateChange;
    public event Action<StanceType> OnStanceChange;
    public event Action<GuardDirection> OnGuardDirectionChange;
    public event Action<bool> OnDodge;
    public float CurrentMoveSpeed { get; private set; }
    public Vector3 CurrentMoveVector => movementDirection.InverseTransformVector(movementVector);
    public float MaxRunSpeed => runSpeed;
    public StanceType CurrentStance => currentStance;
    public GuardDirection CurrentGuardDirection { get; private set; } = GuardDirection.Left;

    [SerializeField] Transform playerModel;
    [SerializeField] Transform followTarget;
    [SerializeField] Transform movementDirection;
    [SerializeField] Transform target;
    [Header("Movement")]
    [SerializeField] float modelTurnSpeed = 60f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float runAcceleration = .5f;
    [SerializeField] float strafeSpeed = 4f;
    [SerializeField] float strafeAcceleration = .5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float dodgeSpeed = 5f;
    [SerializeField] float dodgeTime = .5f;
    [Header("Combat")]
    [SerializeField] float attackLength = 2f;
    [SerializeField] float attackMovementSpeed = 2f;
    [SerializeField] float attackMovementStart = .2f;
    [SerializeField] float attackMovementStop = .7f;
    [SerializeField] float guardChangeSensitivity = 3f;

    Vector3 movementVector;
    Quaternion targetRotation, currentRotation;

    Dictionary<MovementType, MovementState> movementStates;
    MovementType defaultMovementState;

    MovementState currentState;
    StanceType currentStance;

    Vector2 lastMousePosition;

    private void Awake()
    {
        movementStates = new Dictionary<MovementType, MovementState>()
        {
            { MovementType.Standing, new StandingState(this) },
            { MovementType.Running, new RunningState(this) },
            { MovementType.Strafing, new StrafingState(this) },
            { MovementType.Sprinting, new SprintingState(this) },
            { MovementType.Attacking, new AttackingState(this) },
            { MovementType.Dodging, new DodgingState(this) },
        };
        defaultMovementState = MovementType.Standing;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnGuardDirectionChange?.Invoke(CurrentGuardDirection);
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
            MovementType newState = (MovementType)returnedState;
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

    /// <summary>
    /// Move player laterally in world space
    /// </summary>
    /// <param name="moveSpeed"></param>
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

    void MoveInDirection(Vector3 direction, float speed)
    {
        direction.Normalize();
        transform.Translate(speed * Time.deltaTime * direction, Space.Self);
        CurrentMoveSpeed = speed;
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

    void CheckGuardDirection()
    {
        GuardDirection newDirection = CurrentGuardDirection;
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

        if (newDirection != CurrentGuardDirection)
        {
            SetGuardDirection(newDirection);
        }
        lastMousePosition = currentMousePosition;
    }

    void SetGuardDirection(GuardDirection direction)
    {
        if (direction == CurrentGuardDirection)
        {
            return;
        }
        CurrentGuardDirection = direction;
        OnGuardDirectionChange?.Invoke(CurrentGuardDirection);
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
                // dodge
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    return MovementType.Dodging;
                }
                if (movement.CurrentStance == StanceType.Combat)
                {
                    return MovementType.Strafing;
                }
                else
                {
                    return MovementType.Running;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                return MovementType.Attacking;
            }
            return null;
        }

        public override void DuringExecution()
        {
            if (movement.currentStance == StanceType.Combat)
            {
                movement.CombatRotatePlayerModel();
                movement.CheckGuardDirection();
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
            if (Input.GetKey(KeyCode.LeftShift))
            {
                return MovementType.Sprinting;
            }
            if (movement.CurrentStance == StanceType.Combat)
            {
                return MovementType.Strafing;
            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                return MovementType.Standing;
            }
            if (Input.GetMouseButtonDown(0))
            {
                return MovementType.Attacking;
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

    class SprintingState : MovementState
    {
        float currentSprintSpeed;

        public SprintingState(PlayerMovement movement) : base(movement)
        {

        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            currentSprintSpeed = 0f;
            print("Sprinting");
        }

        public override MovementType? CheckTransitions()
        {
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentSprintSpeed = Mathf.Lerp(movement.CurrentMoveSpeed, movement.sprintSpeed, movement.runAcceleration * Time.deltaTime);
            movement.MoveLaterally(currentSprintSpeed);
            movement.RotatePlayerModel();
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class StrafingState : MovementState
    {
        float currentStrafeSpeed = 0f;
        
        public StrafingState(PlayerMovement movement) : base(movement)
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
            // dodge
            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && Input.GetKeyDown(KeyCode.Space))
            {
                return MovementType.Dodging;
            }
            if (Input.GetMouseButtonDown(0))
            {
                return MovementType.Attacking;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                return MovementType.Sprinting;
            }
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
            movement.CheckGuardDirection();
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class AttackingState : MovementState
    {
        float currentAttackLength = 0f;

        public AttackingState(PlayerMovement movement) : base(movement)
        {

        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Attacking");
            currentAttackLength = 0f;
        }

        public override MovementType? CheckTransitions()
        {
            if (currentAttackLength >= movement.attackLength)
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            movement.CheckGuardDirection();
            currentAttackLength += Time.deltaTime;
            if (currentAttackLength >= movement.attackMovementStart && currentAttackLength < movement.attackMovementStop)
            {
                movement.MoveInDirection(movement.playerModel.forward, movement.attackMovementSpeed);
            }
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class DodgingState : MovementState
    {
        bool jumpRight;
        float currentDodgeTime;

        public DodgingState(PlayerMovement movement) : base(movement)
        {

        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Dodging");
            if (Input.GetKey(KeyCode.D))
            {
                jumpRight = true;
                movement?.OnDodge.Invoke(jumpRight);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                jumpRight = false;
                movement?.OnDodge.Invoke(jumpRight);
            }
            currentDodgeTime = 0;
        }

        public override MovementType? CheckTransitions()
        {
            if (currentDodgeTime >= movement.dodgeTime)
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentDodgeTime += Time.deltaTime;
            if (jumpRight)
            {
                movement.MoveInDirection(movement.playerModel.right, movement.dodgeSpeed);
            }
            else
            {
                movement.MoveInDirection(-movement.playerModel.right, movement.dodgeSpeed);
            }
            movement.CombatRotatePlayerModel();
            movement.CheckGuardDirection();
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    #endregion
}
