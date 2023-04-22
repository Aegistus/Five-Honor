using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    public bool Forwards { get; protected set; }
    public bool Backwards { get; protected set; }
    public bool Left { get; protected set; }
    public bool Right { get; protected set; }
    public bool Dodge { get; protected set; }
    public bool LightAttack { get; protected set; }
    public bool Sprint { get; protected set; }

    public abstract GuardDirection GetGuardDirection();
}
