using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentAnimationEvents : MonoBehaviour
{
    public event Action OnRelease;
    public event Action OnFollowThru;
    public event Action OnFinished;

    public void Release()
    {
        OnRelease?.Invoke();
    }

    public void FollowThru()
    {
        OnFollowThru?.Invoke();
    }

    public void Finish()
    {
        OnFinished?.Invoke();
    }
}
