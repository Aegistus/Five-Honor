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

    bool leftIK;
    bool rightIK;
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        SetupWeapons();
    }

    void SetupWeapons()
    {
        // two handed weapon
        if (leftWeapon && rightWeapon && leftWeapon == rightWeapon)
        {
            rightWeapon.transform.SetParent(rightHand);
            rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.rightHandRotation);
            rightWeapon.transform.localPosition = rightWeapon.rightHandOffset;
            leftIK = true;
        }
        else
        {
            if (leftWeapon)
            {
                leftWeapon.transform.SetParent(leftHand);
                leftWeapon.transform.localRotation = Quaternion.Euler(leftWeapon.leftHandRotation);
                leftWeapon.transform.localPosition = leftWeapon.leftHandOffset;
            }
            if (rightWeapon)
            {
                rightWeapon.transform.SetParent(rightHand);
                rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.rightHandRotation);
                rightWeapon.transform.localPosition = rightWeapon.rightHandOffset;
            }
        }
    }

    private void Update()
    {
        if (rightWeapon && leftWeapon && rightWeapon == leftWeapon)
        {
            rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.rightHandRotation);
            rightWeapon.transform.localPosition = rightWeapon.rightHandOffset;
        }
        else
        {
            if (rightWeapon)
            {
                rightWeapon.transform.localRotation = Quaternion.Euler(rightWeapon.rightHandRotation);
                rightWeapon.transform.localPosition = rightWeapon.rightHandOffset;
            }
            if (leftWeapon)
            {
                leftWeapon.transform.localRotation = Quaternion.Euler(leftWeapon.leftHandRotation);
                leftWeapon.transform.localPosition = leftWeapon.leftHandOffset;
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (rightWeapon?.leftIKTarget != null && leftIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, rightWeapon.leftIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, rightWeapon.leftIKTarget.rotation);
        }
        else if (leftWeapon?.rightIKTarget != null && rightIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.RightHand, leftWeapon.rightIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, leftWeapon.rightIKTarget.rotation);
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
