using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Hand
{
    None, LeftHand, RightHand
}

public class PlayerIK : MonoBehaviour
{
    [SerializeField] WeaponIK currentWeapon;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;

    Transform WeaponTransform => currentWeapon.transform;
    Hand weaponParent = Hand.None;

    bool ik;
    Animator anim;
    PlayerMovement movement;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        movement = GetComponentInParent<PlayerMovement>();
        movement.OnStanceChange += WeaponChangeHands;
    }

    private void Start()
    {
        WeaponChangeHands(StanceType.Passive);   
    }

    private void WeaponChangeHands(StanceType newStance)
    {
        if (newStance == StanceType.Combat)
        {
            ik = true;
            WeaponTransform.SetParent(rightHand);
            weaponParent = Hand.RightHand;
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.rightHandRotation);
            WeaponTransform.localPosition = currentWeapon.rightHandOffset;
        }
        else if (newStance == StanceType.Passive)
        {
            ik = false;
            WeaponTransform.SetParent(leftHand);
            weaponParent = Hand.LeftHand;
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.leftHandRotation);
            WeaponTransform.localPosition = currentWeapon.leftHandOffset;
        }
    }

    private void Update()
    {
        if (weaponParent == Hand.LeftHand)
        {
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.leftHandRotation);
            WeaponTransform.localPosition = currentWeapon.leftHandOffset;
        }
        if (weaponParent == Hand.RightHand)
        {
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.rightHandRotation);
            WeaponTransform.localPosition = currentWeapon.rightHandOffset;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (WeaponTransform != null && ik)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, currentWeapon.leftIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, currentWeapon.leftIKTarget.rotation);
        }
    }
}
