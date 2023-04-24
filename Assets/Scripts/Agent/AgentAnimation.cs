using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimation : MonoBehaviour
{
    [SerializeField] float transitionTime = .5f;
    [SerializeField] float stanceTransitionSpeed = 2f;

    /// <summary>
    /// 1 = Combat Stance, 2 = Passive Stance
    /// </summary>
    int currentStanceLayer;
    Dictionary<StanceType, int> stanceToLayer = new Dictionary<StanceType, int>()
    {
        {StanceType.Combat, 1},
        {StanceType.Passive, 2 }
    };
    AgentMovement movement;
    Animator anim;

    Dictionary<MovementType, int> movementTypeToAnimation = new Dictionary<MovementType, int>()
    {
        { MovementType.Standing, Animator.StringToHash("Standing") },
        { MovementType.Running, Animator.StringToHash("Running") },
        { MovementType.Strafing, Animator.StringToHash("Strafing") },
        { MovementType.Sprinting, Animator.StringToHash("Sprinting") },
        { MovementType.Attacking, Animator.StringToHash("Attacking") },
        { MovementType.Dodging, Animator.StringToHash("Dodging") },
        { MovementType.Flinching, Animator.StringToHash("Flinching") },
        { MovementType.Blocking, Animator.StringToHash("Blocking") },
    };

    int xMovementHash = Animator.StringToHash("XMovement");
    int zMovementHash = Animator.StringToHash("ZMovement");
    int guardDirectionHash = Animator.StringToHash("GuardDirection");
    int dodgeDirectionHash = Animator.StringToHash("DodgeRight");

    private void Awake()
    {
        movement = GetComponent<AgentMovement>();
        anim = GetComponentInChildren<Animator>();
        movement.OnMovementStateChange += UpdateMovementAnimation;
        movement.OnStanceChange += ChangeStanceAnimations;
        movement.OnGuardDirectionChange += ChangeGuardStance;
        movement.OnDodge += (bool value) => anim.SetBool(dodgeDirectionHash, value);
        ChangeStanceAnimations(StanceType.Passive);
    }

    private void Update()
    {
        anim.SetFloat(xMovementHash, movement.CurrentMoveVector.x);
        anim.SetFloat(zMovementHash, movement.CurrentMoveVector.z);
    }

    void UpdateMovementAnimation(MovementType newMovement)
    {
        print("Changing animation");
        anim.CrossFade(movementTypeToAnimation[newMovement], transitionTime, currentStanceLayer);
    }

    void ChangeStanceAnimations(StanceType newStance)
    {
        if (newStance == StanceType.Combat)
        {
            currentStanceLayer = stanceToLayer[StanceType.Combat];
            StartCoroutine(ChangeStanceLayerWeight(currentStanceLayer, stanceToLayer[StanceType.Passive]));
        }
        else
        {
            currentStanceLayer = stanceToLayer[StanceType.Passive];
            StartCoroutine(ChangeStanceLayerWeight(currentStanceLayer, stanceToLayer[StanceType.Combat]));
        }
        UpdateMovementAnimation(movement.CurrentStateType);
    }

    IEnumerator ChangeStanceLayerWeight(int newLayer, int oldLayer)
    {
        bool newLayerDone = false;
        bool oldLayerDone = false;
        while (!newLayerDone || !oldLayerDone)
        {
            float newLayerWeight = anim.GetLayerWeight(newLayer);
            float oldLayerWeight = anim.GetLayerWeight(oldLayer);
            if (newLayerWeight < 1)
            {
                anim.SetLayerWeight(newLayer, newLayerWeight + (Time.deltaTime * stanceTransitionSpeed));
            }
            else
            {
                anim.SetLayerWeight(newLayer, 1);
                newLayerDone = true;
            }
            if (oldLayerWeight > 0)
            {
                anim.SetLayerWeight(oldLayer, oldLayerWeight - (Time.deltaTime * stanceTransitionSpeed));
            }
            else
            {
                anim.SetLayerWeight(oldLayer, 0);
                oldLayerDone = true;
            }
            yield return null;
        }
    }

    private void ChangeGuardStance(GuardDirection direction)
    {
        anim.SetInteger(guardDirectionHash, (int)direction);
    }
}
