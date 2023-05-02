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
    [SerializeField] float iKTransitionSpeed = 5f;

    bool leftIK;
    bool rightIK;
    float leftIKTargetWeight = 0f;
    float rightIKTargetWeight = 0f;
    float leftIKCurrentWeight = 0f;
    float rightIKCurrentWeight = 0f;
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        rightWeapon = GetComponentInParent<AgentWeapons>().RightWeapon.GetComponent<WeaponIK>();
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
            SetHandIK(Hand.LeftHand, true);
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
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftIKCurrentWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftIKCurrentWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, rightWeapon.leftIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, rightWeapon.leftIKTarget.rotation);
        }
        else if (leftWeapon?.rightIKTarget != null && rightIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightIKCurrentWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightIKCurrentWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, leftWeapon.rightIKTarget.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, leftWeapon.rightIKTarget.rotation);
        }
        if (leftIKCurrentWeight != leftIKTargetWeight)
        {
            leftIKCurrentWeight = Mathf.Lerp(leftIKCurrentWeight, leftIKTargetWeight, iKTransitionSpeed * Time.deltaTime);
        }
        if (rightIKCurrentWeight != rightIKTargetWeight)
        {
            rightIKCurrentWeight = Mathf.Lerp(rightIKCurrentWeight, rightIKTargetWeight, iKTransitionSpeed * Time.deltaTime);
        }
    }

    public void SetHandIK(Hand hand, bool ik)
    {
        if (hand == Hand.LeftHand)
        {
            leftIK = ik;
            leftIKTargetWeight = ik ? 1f : 0f;
        }
        else if (hand == Hand.RightHand)
        {
            rightIK = ik;
            rightIKTargetWeight = ik ? 1f : 0f;
        }
    }
}
