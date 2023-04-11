using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] float transitionTime = .5f;

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
    }

    void UpdateMovementAnimation(MovementType newMovement)
    {
        print("Changing animation");
        anim.CrossFade(movementTypeToAnimation[newMovement], transitionTime, 0);
    }
}
