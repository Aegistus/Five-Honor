using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStateType
{
    Idling, Moving
}

public class EnemyAIController : AgentController
{
    [SerializeField] Transform lookDirection;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float losCheckInterval = .5f;
    [SerializeField] float losCheckRadius = 10f;

    NavMeshAgent navAgent;
    AIState currentState;
    Dictionary<AIStateType, AIState> availableStates;

    private void Awake()
    {
        availableStates = new Dictionary<AIStateType, AIState>()
        {
            { AIStateType.Idling, new AIIdleState(this) },
            { AIStateType.Moving, new AIMovingState(this) },
        };
        SetAIState(AIStateType.Idling);
    }

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = 0f;
        // testing
        SetDestination(new Vector3(-1.31f, 1.85f, 32.62f));
    }

    public void SetDestination(Vector3 position)
    {
        navAgent.SetDestination(position);
    }

    void SetAIState(AIStateType newState)
    {
        if (currentState != null)
        {
            currentState.AfterExecution();
        }
        currentState = availableStates[newState];
        currentState.BeforeExecution();
    }

    private void Update()
    {
        currentState.DuringExecution();
        AIStateType? returnedState = currentState.CheckTransitions();
        if (returnedState != null)
        {
            AIStateType nextState = (AIStateType)returnedState;
            SetAIState(nextState);
        }
    }

    public override void FindNewTarget()
    {

    }

    public override GuardDirection GetGuardDirection()
    {
        return GuardDirection.None;
    }

    abstract class AIState
    {
        protected Transform transform;
        protected GameObject gameObject;

        protected EnemyAIController controller;
        public AIState(EnemyAIController controller)
        {
            this.controller = controller;
            transform = controller.transform;
            gameObject = controller.gameObject;
        }

        public abstract void BeforeExecution();
        public abstract void DuringExecution();
        public abstract  void AfterExecution();
        public abstract AIStateType? CheckTransitions();
    }

    class AIIdleState : AIState
    {
        public AIIdleState(EnemyAIController controller) : base(controller) { }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {

        }

        public override AIStateType? CheckTransitions()
        {
            if (controller.navAgent.hasPath)
            {
                return AIStateType.Moving;
            }
            return null;
        }

        public override void DuringExecution()
        {

        }
    }

    class AIMovingState : AIState
    {
        Vector3 nextDestination;
        float arrivedDistance = .2f;

        public AIMovingState(EnemyAIController controller) : base(controller) { }

        public override void AfterExecution()
        {
            controller.Forwards = false;
        }

        public override void BeforeExecution()
        {
            print("Moving");
            nextDestination = controller.navAgent.path.corners[0];
        }

        public override void DuringExecution()
        {
            if (Vector3.Distance(transform.position, nextDestination) <= arrivedDistance && controller.navAgent.hasPath)
            {
                print("Changing Destination");
                nextDestination = controller.navAgent.path.corners[0];
            }
            controller.lookDirection.LookAt(nextDestination);
            controller.Forwards = true;
        }

        public override AIStateType? CheckTransitions()
        {
            if (!controller.navAgent.hasPath)
            {
                return AIStateType.Idling;
            }
            return null;
        }
    }
}
