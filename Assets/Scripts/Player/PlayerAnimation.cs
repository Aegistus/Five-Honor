using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] float transitionTime = .5f;

    /// <summary>
    /// 0 = Combat Stance, 1 = Passive Stance
    /// </summary>
    int currentStanceLayer = 0;
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
            currentStanceLayer = 0;
            StartCoroutine(ChangeStanceLayerWeight(currentStanceLayer, 1));
        }
        else
        {
            currentStanceLayer = 1;
            StartCoroutine(ChangeStanceLayerWeight(currentStanceLayer, 0));
        }
        UpdateMovementAnimation(movement.CurrentStateType);
    }

    IEnumerator ChangeStanceLayerWeight(int newLayer, int oldLayer)
    {
        bool newLayerDone = false;
        bool oldLayerDone = false;
        while (!newLayerDone || !oldLayerDone)
        {
            yield return null;
            float newLayerWeight = anim.GetLayerWeight(newLayer);
            float oldLayerWeight = anim.GetLayerWeight(oldLayer);
            if (newLayerWeight < 1)
            {
                anim.SetLayerWeight(newLayer, newLayerWeight + Time.deltaTime);
            }
            else
            {
                newLayerDone = true;
            }
            if (oldLayerWeight > 0)
            {
                anim.SetLayerWeight(oldLayer, oldLayerWeight - Time.deltaTime);
            }
            else
            {
                oldLayerDone = true;
            }
        }
    }
}
