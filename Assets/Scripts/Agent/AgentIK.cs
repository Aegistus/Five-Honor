using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hand
{
    None, LeftHand, RightHand
}

public class AgentIK : MonoBehaviour
{
    [SerializeField] WeaponIK leftWeapon;
    [SerializeField] WeaponIK rightWeapon;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    [SerializeField] bool switchHandInPassive = false;

    bool leftIK;
    bool rightIK;
    Animator anim;
    AgentMovement movement;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        movement = GetComponentInParent<AgentMovement>();
        movement.OnStanceChange += WeaponChangeHands;
    }

    private void Start()
    {
        WeaponChangeHands(StanceType.Passive);   
    }

    private void WeaponChangeHands(StanceType newStance)
    {
        if (leftWeapon)
        {
            leftWeapon.transform.SetParent(leftHand);
            leftWeapon.transform.localRotation = Quaternion.Euler(leftWeapon.leftHandRotation);
            leftWeapon.transform.localPosition = leftWeapon.leftHandOffset;
        }
        if (rightWeapon)
        {
            if (newStance == StanceType.Combat && rightWeapon.leftIKTarget)
            {
                leftIK = true;
            }
            else
            {
                leftIK = false;
            }
            rightWeapon.transform.SetParent(rightHand);
            rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.rightHandRotation);
            rightWeapon.transform.localPosition = rightWeapon.rightHandOffset;
        }

    }

    private void Update()
    {
        if (rightWeapon)
        {
            UpdateWeaponTransformToHand(rightWeapon, Hand.RightHand);

        }
        if (leftWeapon)
        {
            UpdateWeaponTransformToHand(leftWeapon, Hand.LeftHand);
        }
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

    public void SetHandIK(Hand hand, bool ik)
    {
        if (hand == Hand.LeftHand)
        {
            leftIK = ik;
        }
        else if (hand == Hand.RightHand)
        {
            rightIK = ik;
        }
    }
}
