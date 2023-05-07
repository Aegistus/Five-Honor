using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStateType
{
    Wandering, Moving
}

public class EnemyAIController : AgentController
{
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
            { AIStateType.Moving, new AIMovingState(this) },
        };
    }

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = 0f;
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
        protected EnemyAIController controller;
        public AIState(EnemyAIController controller)
        {
            this.controller = controller;
        }

        public abstract void BeforeExecution();
        public abstract void DuringExecution();
        public abstract  void AfterExecution();
        public abstract AIStateType? CheckTransitions();
    }

    class AIMovingState : AIState
    {
        public AIMovingState(EnemyAIController controller) : base(controller) { }

        public override void AfterExecution()
        {

        }

        public override void BeforeExecution()
        {

        }

        public override void DuringExecution()
        {

        }

        public override AIStateType? CheckTransitions()
        {
            return null;
        }
    }
}
