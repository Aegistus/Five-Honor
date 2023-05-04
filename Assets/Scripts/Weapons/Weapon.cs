using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    public event Action OnAttackBlocked;
    public AnimatorOverrideController AnimationSet => animationSet;

    [SerializeField] float damage = 10f;
    [SerializeField] AnimatorOverrideController animationSet;

    bool inDamageState = false;
    GuardDirection attackDirection = GuardDirection.None;
    List<AgentHealth> alreadyHit = new List<AgentHealth>();
    AgentHealth damageSource;
    int hitSoundID;
    int swingSoundID;

    private void Start()
    {
        hitSoundID = SoundManager.Instance.GetSoundID("Sword_Hit");
        swingSoundID = SoundManager.Instance.GetSoundID("Sword_Swing");
    }

    public void EnterDamageState(GuardDirection direction, AgentHealth damageSource)
    {
        this.damageSource = damageSource;
        attackDirection = direction;
        inDamageState = true;
        alreadyHit.Clear();
        SoundManager.Instance.PlaySoundAtPosition(swingSoundID, transform.position);
    }

    public void ExitDamageState()
    {
        inDamageState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inDamageState)
        {
            AgentHealth health = other.GetComponentInParent<AgentHealth>();
            if (health != null && !alreadyHit.Contains(health) && health != damageSource && !health.IsDead)
            {
                bool attackSucceeded = health.AttemptDamage(damage, attackDirection);
                if (!attackSucceeded)
                {
                    OnAttackBlocked?.Invoke();
                    inDamageState = false;
                }
                SoundManager.Instance.PlaySoundAtPosition(hitSoundID, transform.position);
                alreadyHit.Add(health);
            }
        }
    }
}
