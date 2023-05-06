using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    public event Action OnAttackBlocked;
    public AnimatorOverrideController AnimationSet => animationSet;
    public float StaminaCost => staminaCost;

    [SerializeField] float damage = 10f;
    [SerializeField] float knockbackForce = 1f;
    [SerializeField] float knockbackTime = 1f;
    [SerializeField] float staminaCost = 30f;
    [SerializeField] AnimatorOverrideController animationSet;

    bool inDamageState = false;
    GuardDirection attackDirection = GuardDirection.None;
    List<AgentHealth> alreadyHit = new List<AgentHealth>();
    AgentHealth damageSource;
    float timer = 0f;
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
            HitTarget(other.GetComponentInParent<AgentHealth>());
        }
    }

    void HitTarget(AgentHealth health)
    {
        if (health != null && !alreadyHit.Contains(health) && health != damageSource && !health.IsDead)
        {
            bool attackSucceeded = health.AttemptDamage(damage, attackDirection);
            if (!attackSucceeded)
            {
                OnAttackBlocked?.Invoke();
                inDamageState = false;
            }
            else
            {
                timer = knockbackTime;
                AgentMovement target = health.GetComponent<AgentMovement>();
                if (target != null)
                {
                    StartCoroutine(Knockback(target));
                }
            }
            SoundManager.Instance.PlaySoundAtPosition(hitSoundID, transform.position);
            alreadyHit.Add(health);
        }
    }

    IEnumerator Knockback(AgentMovement target)
    {
        Vector3 knockbackDirection = (target.transform.position - damageSource.transform.position).normalized;
        knockbackDirection.y = 0f;
        while (timer > 0)
        {
            target.MoveInDirection(target.AgentModel.InverseTransformDirection(knockbackDirection), knockbackForce);
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
