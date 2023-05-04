using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentAnimationEvents : MonoBehaviour
{
    public event Action OnAttackRelease;
    public event Action OnAttackFollowThru;
    public event Action OnAttackFinished;

    public void Release()
    {
        OnAttackRelease?.Invoke();
    }

    public void FollowThru()
    {
        OnAttackFollowThru?.Invoke();
    }

    public void Finish()
    {
        OnAttackFinished?.Invoke();
    }
}
