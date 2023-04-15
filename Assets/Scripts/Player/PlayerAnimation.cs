using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
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
    PlayerMovement movement;
    Animator anim;

    Dictionary<MovementType, int> movementTypeToAnimation = new Dictionary<MovementType, int>()
    {
        {MovementType.Standing, Animator.StringToHash("Standing") },
        {MovementType.Running, Animator.StringToHash("Running") },
    };

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        anim = GetComponentInChildren<Animator>();
        movement.OnMovementStateChange += UpdateMovementAnimation;
        movement.OnStanceChange += ChangeStanceAnimations;
        ChangeStanceAnimations(StanceType.Passive);
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
}
