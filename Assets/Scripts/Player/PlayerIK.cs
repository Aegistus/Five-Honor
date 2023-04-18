using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HandParent
{
    None, LeftHand, RightHand
}

public class PlayerIK : MonoBehaviour
{
    [SerializeField] WeaponIK currentWeapon;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;

    Transform WeaponTransform => currentWeapon.transform;
    HandParent weaponParent = HandParent.None;

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
            weaponParent = HandParent.RightHand;
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.rightHandRotation);
            WeaponTransform.localPosition = currentWeapon.rightHandOffset;
        }
        else if (newStance == StanceType.Passive)
        {
            ik = false;
            WeaponTransform.SetParent(leftHand);
            weaponParent = HandParent.LeftHand;
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.leftHandRotation);
            WeaponTransform.localPosition = currentWeapon.leftHandOffset;
        }
    }

    private void Update()
    {
        if (weaponParent == HandParent.LeftHand)
        {
            WeaponTransform.localRotation = Quaternion.Euler(currentWeapon.leftHandRotation);
            WeaponTransform.localPosition = currentWeapon.leftHandOffset;
        }
        if (weaponParent == HandParent.RightHand)
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
            anim.SetIKPosition(AvatarIKGoal.LeftHand, WeaponTransform.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, WeaponTransform.rotation);
        }
    }
}
