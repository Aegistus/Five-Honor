using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Hand
{
    None, LeftHand, RightHand
}

public class PlayerIK : MonoBehaviour
{
    [SerializeField] WeaponIK leftWeapon;
    [SerializeField] WeaponIK rightWeapon;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    [SerializeField] bool switchHandInPassive = false;

    Hand rightWeaponParent = Hand.None;
    Hand leftWeaponParent = Hand.None;

    bool leftIK;
    bool rightIK;
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
            if (rightWeapon.leftIKTarget)
            {
                leftIK = true;
            }
            rightWeapon.transform.SetParent(rightHand);
            rightWeaponParent = Hand.RightHand;
            rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.rightHandRotation);
            rightWeapon.transform.localPosition = rightWeapon.rightHandOffset;
            if (leftWeapon)
            {
                leftWeapon.transform.SetParent(leftHand);
                leftWeaponParent = Hand.LeftHand;
                leftWeapon.transform.localRotation = Quaternion.Euler(leftWeapon.leftHandRotation);
                leftWeapon.transform.localPosition = leftWeapon.leftHandOffset;
            }
        }
        else if (newStance == StanceType.Passive)
        {
            leftIK = false;
            rightWeapon.transform.SetParent(leftHand);
            rightWeaponParent = Hand.LeftHand;
            rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.leftHandRotation);
            rightWeapon.transform.localPosition = rightWeapon.leftHandOffset;
            if (leftWeapon)
            {
                leftWeapon.transform.SetParent(leftHand);
                leftWeaponParent = Hand.LeftHand;
                leftWeapon.transform.localRotation = Quaternion.Euler(leftWeapon.leftHandRotation);
                leftWeapon.transform.localPosition = leftWeapon.leftHandOffset;
            }
        }
    }

    private void Update()
    {
        UpdateWeaponTransformToHand(rightWeapon, Hand.RightHand);
        UpdateWeaponTransformToHand(leftWeapon, Hand.LeftHand);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (rightWeapon != null && leftIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, rightWeapon.leftIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, rightWeapon.leftIKTarget.rotation);
        }
        else if (leftWeapon != null && rightIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.RightHand, leftWeapon.rightIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, leftWeapon.rightIKTarget.rotation);
        }
    }

    void UpdateWeaponTransformToHand(WeaponIK weapon, Hand hand)
    {
        if (hand == Hand.RightHand)
        {
            weapon.transform.localRotation = Quaternion.Euler(weapon.rightHandRotation);
            weapon.transform.localPosition = weapon.rightHandOffset;
        }
        else if (hand == Hand.LeftHand)
        {
            weapon.transform.localRotation = Quaternion.Euler(weapon.leftHandRotation);
            weapon.transform.localPosition = weapon.leftHandOffset;
        }
    }
}
