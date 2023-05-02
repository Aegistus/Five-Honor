using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MovementType
{
    Standing, Running, Sprinting, Strafing, Attacking, Dodging, Flinching, Blocking, SprintAttack
}
public enum StanceType
{
    Passive, Combat
}


public class AgentMovement : MonoBehaviour
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

    [SerializeField] Transform agentModel;
    [SerializeField] Transform followTarget;
    [SerializeField] Transform movementDirection;
    [SerializeField] Transform target;
    [Header("Movement")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundOffset;
    [SerializeField] float modelTurnSpeed = 60f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float runAcceleration = .5f;
    [SerializeField] float strafeSpeed = 4f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float dodgeSpeed = 5f;
    [SerializeField] float dodgeTime = .5f;
    [Header("Combat")]
    [SerializeField] float attackLength = 2f;
    [SerializeField] float windupDuration = 1f;
    [SerializeField] float attackMovementSpeed = 2f;
    [SerializeField] float attackMovementStart = .2f;
    [SerializeField] float attackMovementStop = .7f;
    [SerializeField] float flinchDuration = 1f;
    [SerializeField] float blockDuration = 1f;
    [SerializeField] float sprintAttackMovementSpeed = 4f;

    AgentController controller;
    AgentWeapons agentWeapons;
    AgentHealth agentHealth;
    AgentIK agentIK;

    Vector3 movementVector;
    Quaternion targetRotation, currentRotation;

    Dictionary<MovementType, MovementState> movementStates;
    MovementType defaultMovementState;

    MovementState currentState;
    StanceType currentStance;

    private void Awake()
    {
        controller = GetComponent<AgentController>();
        agentWeapons = GetComponent<AgentWeapons>();
        agentHealth = GetComponent<AgentHealth>();
        agentIK = GetComponentInChildren<AgentIK>();
        agentHealth.OnDamageTaken += () => ChangeState(MovementType.Flinching);
        agentHealth.OnDamageBlocked += () => ChangeState(MovementType.Blocking);
        movementStates = new Dictionary<MovementType, MovementState>()
        {
            { MovementType.Standing, new StandingState(this) },
            { MovementType.Running, new RunningState(this) },
            { MovementType.Strafing, new StrafingState(this) },
            { MovementType.Sprinting, new SprintingState(this) },
            { MovementType.Attacking, new AttackingState(this) },
            { MovementType.Dodging, new DodgingState(this) },
            { MovementType.Flinching, new FlinchingState(this) },
            { MovementType.Blocking, new BlockingState(this) },
            { MovementType.SprintAttack, new SprintAttackingState(this) },
        };
        defaultMovementState = MovementType.Standing;
    }

    private void Start()
    {
        OnGuardDirectionChange?.Invoke(CurrentGuardDirection);
    }

    private void Update()
    {
        // change stance
        if (controller.StanceChange)
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
            ChangeState(newState);
        }
        currentState.DuringExecution();

        StickAgentToGround();
    }

    private void FixedUpdate()
    {
        currentState?.DuringPhysicsUpdate();
    }

    void ChangeState(MovementType newState)
    {
        currentState.AfterExecution();
        currentState = movementStates[newState];
        currentState.BeforeExecution();
        CurrentStateType = newState;
        OnMovementStateChange?.Invoke(CurrentStateType);
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
        if (controller.Forwards)
        {
            movementVector += movementDirection.forward;
        }
        if (controller.Backwards)
        {
            movementVector -= movementDirection.forward;
        }
        if (controller.Left)
        {
            movementVector -= movementDirection.right;
        }
        if (controller.Right)
        {
            movementVector += movementDirection.right;
        }
        movementVector.Normalize();
        transform.Translate(moveSpeed * Time.deltaTime * movementVector, Space.World);
        CurrentMoveSpeed = moveSpeed;
    }

    /// <summary>
    /// Takes direction relative to agent model.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    void MoveInDirection(Vector3 direction, float speed)
    {
        direction.Normalize();
        direction = agentModel.TransformVector(direction);
        transform.Translate(speed * Time.deltaTime * direction, Space.World);
        CurrentMoveSpeed = speed;
    }

    void CombatRotateAgentModel()
    {
        currentRotation = agentModel.rotation;
        agentModel.LookAt(target);
        targetRotation.eulerAngles = new Vector3(0, agentModel.eulerAngles.y, 0);
        agentModel.rotation = currentRotation;
        agentModel.rotation = Quaternion.Lerp(currentRotation, targetRotation, modelTurnSpeed * Time.deltaTime);
    }

    void RotateAgentModel()
    {
        // model rotation
        currentRotation = agentModel.rotation;
        agentModel.LookAt(agentModel.position + movementVector);
        targetRotation.eulerAngles = new Vector3(0, agentModel.eulerAngles.y, 0);
        agentModel.rotation = currentRotation;
        agentModel.rotation = Quaternion.Lerp(currentRotation, targetRotation, modelTurnSpeed * Time.deltaTime);
    }

    void ChangeStance(StanceType newStance)
    {
        currentStance = newStance;
        OnStanceChange?.Invoke(newStance);
    }

    void SetGuardDirection(GuardDirection direction)
    {
        if (direction == CurrentGuardDirection || direction == GuardDirection.None)
        {
            return;
        }
        CurrentGuardDirection = direction;
        OnGuardDirectionChange?.Invoke(CurrentGuardDirection);
    }

    void StickAgentToGround()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(agentModel.position + Vector3.up, Vector3.down, out rayHit, 5f, groundLayer))
        {
            Vector3 adjustedPosition = new Vector3(transform.position.x, rayHit.point.y + groundOffset, transform.position.z);
            transform.position = adjustedPosition;
        }
    }

    #region Movement States
    abstract class MovementState
    {
        protected AgentMovement movement;

        public MovementState(AgentMovement movement)
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
        public StandingState(AgentMovement movement) : base(movement) { }

        public override void AfterExecution()
        {
            if (movement.CurrentStance == StanceType.Passive)
            {
                movement.agentIK.SetHandIK(Hand.LeftHand, true);
            }
        }

        public override void BeforeExecution()
        {
            print("Standing");
            if (movement.CurrentStance == StanceType.Passive)
            {
                movement.agentIK.SetHandIK(Hand.LeftHand, false);
            }
        }

        public override MovementType? CheckTransitions()
        {
            if (movement.controller.MovementInput)
            {
                // dodge
                if (movement.controller.Dodge)
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
            if (movement.controller.LightAttack)
            {
                return MovementType.Attacking;
            }
            return null;
        }

        public override void DuringExecution()
        {
            if (movement.currentStance == StanceType.Combat)
            {
                movement.CombatRotateAgentModel();
                movement.SetGuardDirection(movement.controller.GetGuardDirection());
            }
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class RunningState : MovementState
    {
        float currentRunSpeed = 0f;

        public RunningState(AgentMovement movement) : base(movement) { }

        public override void AfterExecution()
        {
            movement.movementVector = Vector3.zero;
        }

        public override void BeforeExecution()
        {
            print("Running");
            currentRunSpeed = 0;
        }

        public override MovementType? CheckTransitions()
        {
            if (movement.controller.Sprint)
            {
                return MovementType.Sprinting;
            }
            if (movement.CurrentStance == StanceType.Combat)
            {
                return MovementType.Strafing;
            }
            if (movement.controller.NoMovementInput)
            {
                return MovementType.Standing;
            }
            if (movement.controller.LightAttack)
            {
                return MovementType.Attacking;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentRunSpeed = Mathf.Lerp(movement.CurrentMoveSpeed, movement.runSpeed, movement.runAcceleration * Time.deltaTime);
            movement.MoveLaterally(currentRunSpeed);
            movement.RotateAgentModel();
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class SprintingState : MovementState
    {
        float currentSprintSpeed;

        public SprintingState(AgentMovement movement) : base(movement)
        {

        }

        public override void AfterExecution()
        {
            movement.movementVector = Vector3.zero;
        }

        public override void BeforeExecution()
        {
            currentSprintSpeed = 0f;
            print("Sprinting");
        }

        public override MovementType? CheckTransitions()
        {
            if (movement.controller.NoMovementInput)
            {
                return MovementType.Standing;
            }
            if (movement.controller.LightAttack)
            {
                return MovementType.SprintAttack;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentSprintSpeed = Mathf.Lerp(movement.CurrentMoveSpeed, movement.sprintSpeed, movement.runAcceleration * Time.deltaTime);
            movement.MoveLaterally(currentSprintSpeed);
            movement.RotateAgentModel();
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class StrafingState : MovementState
    {
        float currentStrafeSpeed = 0f;
        KeyCode currentKeyDirection;
        
        public StrafingState(AgentMovement movement) : base(movement)
        {
        }

        public override void AfterExecution()
        {
            movement.movementVector = Vector3.zero;
        }

        public override void BeforeExecution()
        {
            print("Strafing");
            currentStrafeSpeed = 0f;
        }

        public override MovementType? CheckTransitions()
        {
            // dodge
            if ((movement.controller.Left || movement.controller.Right) && movement.controller.Dodge)
            {
                return MovementType.Dodging;
            }
            if (movement.controller.LightAttack)
            {
                return MovementType.Attacking;
            }
            if (movement.controller.Sprint)
            {
                return MovementType.Sprinting;
            }
            if (movement.currentStance != StanceType.Combat)
            {
                return MovementType.Running;
            }
            if (movement.controller.NoMovementInput)
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentStrafeSpeed = Mathf.Lerp(movement.CurrentMoveSpeed, movement.strafeSpeed, movement.runAcceleration * Time.deltaTime);
            movement.MoveLaterally(currentStrafeSpeed);
            movement.CombatRotateAgentModel();
            movement.SetGuardDirection(movement.controller.GetGuardDirection());
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class AttackingState : MovementState
    {
        float currentDuration = 0f;
        bool attackCanceled = false;
        bool attackReleased = false;
        Vector3 movementDirection;

        public AttackingState(AgentMovement movement) : base(movement)
        {

        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Attacking");
            attackCanceled = false;
            attackReleased = false;
            movement.agentWeapons.RightWeapon.OnAttackBlocked += AttackCanceled;
            movement.agentHealth.OnDamageTaken += AttackCanceled;
            currentDuration = 0f;
            if (movement.controller.Forwards)
            {
                movementDirection = Vector3.forward;
            }
            else if (movement.controller.Backwards)
            {
                movementDirection = Vector3.back;
            }
            else if (movement.controller.Left)
            {
                movementDirection = Vector3.left;
            }
            else if (movement.controller.Right)
            {
                movementDirection = Vector3.right;
            }
            else
            {
                movementDirection = Vector3.zero;
            }
        }

        private void AttackCanceled()
        {
            attackCanceled = true;
        }

        public override MovementType? CheckTransitions()
        {
            if (attackCanceled)
            {
                return MovementType.Standing;
            }
            if (currentDuration >= movement.attackLength)
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            movement.SetGuardDirection(movement.controller.GetGuardDirection());
            currentDuration += Time.deltaTime;
            if (currentDuration >= movement.attackMovementStart && currentDuration < movement.attackMovementStop)
            {
                movement.MoveInDirection(movementDirection, movement.attackMovementSpeed);
            }
            if (currentDuration >= movement.windupDuration && !attackReleased)
            {
                movement.agentWeapons.Attack(movement.attackLength, movement.CurrentGuardDirection);
                attackReleased = true;
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

        public DodgingState(AgentMovement movement) : base(movement)
        {

        }

        public override void AfterExecution()
        {
            movement.agentIK.SetHandIK(Hand.LeftHand, true);
        }

        public override void BeforeExecution()
        {
            print("Dodging");
            if (movement.controller.Right)
            {
                jumpRight = true;
                movement?.OnDodge.Invoke(jumpRight);
            }
            else if (movement.controller.Left)
            {
                jumpRight = false;
                movement?.OnDodge.Invoke(jumpRight);
            }
            currentDodgeTime = 0;
            movement.agentIK.SetHandIK(Hand.LeftHand, false);
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
                movement.MoveInDirection(Vector3.right, movement.dodgeSpeed);
            }
            else
            {
                movement.MoveInDirection(Vector3.left, movement.dodgeSpeed);
            }
            movement.CombatRotateAgentModel();
            movement.SetGuardDirection(movement.controller.GetGuardDirection());
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class FlinchingState : MovementState
    {
        float currentTime;

        public FlinchingState(AgentMovement movement) : base(movement)
        {
        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Flinching");
            currentTime = 0f;
        }

        public override MovementType? CheckTransitions()
        {
            if (currentTime >= movement.flinchDuration)
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            currentTime += Time.deltaTime;
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class BlockingState : MovementState
    {
        float currentTime = 0f;

        public BlockingState(AgentMovement movement) : base(movement)
        {
        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            currentTime = 0f;
        }

        public override MovementType? CheckTransitions()
        {
            if (currentTime >= movement.blockDuration)
            {
                return MovementType.Standing;
            }

            return null;
        }

        public override void DuringExecution()
        {
            currentTime += Time.deltaTime;
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }

    class SprintAttackingState : MovementState
    {
        float currentDuration = 0f;
        bool attackCanceled = false;
        bool attackReleased = false;
        Vector3 movementDirection;

        public SprintAttackingState(AgentMovement movement) : base(movement)
        {
        }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {
            print("Attacking");
            attackCanceled = false;
            attackReleased = false;
            movement.agentWeapons.RightWeapon.OnAttackBlocked += AttackCanceled;
            currentDuration = 0f;
            if (movement.controller.Forwards)
            {
                movementDirection = Vector3.forward;
            }
            else if (movement.controller.Backwards)
            {
                movementDirection = Vector3.back;
            }
            else if (movement.controller.Left)
            {
                movementDirection = Vector3.left;
            }
            else if (movement.controller.Right)
            {
                movementDirection = Vector3.right;
            }
            else
            {
                movementDirection = Vector3.zero;
            }
        }

        private void AttackCanceled()
        {
            attackCanceled = true;
        }

        public override MovementType? CheckTransitions()
        {
            if (attackCanceled)
            {
                return MovementType.Standing;
            }
            if (currentDuration >= movement.attackLength)
            {
                return MovementType.Standing;
            }
            return null;
        }

        public override void DuringExecution()
        {
            movement.SetGuardDirection(movement.controller.GetGuardDirection());
            currentDuration += Time.deltaTime;
            if (currentDuration >= movement.attackMovementStart && currentDuration < movement.attackMovementStop)
            {
                movement.MoveInDirection(movementDirection, movement.sprintAttackMovementSpeed);
            }
            if (currentDuration >= movement.windupDuration && !attackReleased)
            {
                movement.agentWeapons.Attack(movement.attackLength, movement.CurrentGuardDirection);
                attackReleased = true;
            }
        }

        public override void DuringPhysicsUpdate()
        {

        }
    }
    #endregion
}
