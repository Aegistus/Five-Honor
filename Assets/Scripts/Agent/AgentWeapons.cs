using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentWeapons : MonoBehaviour
{
    [SerializeField] Weapon rightWeapon;

    public Weapon RightWeapon => rightWeapon;

    AgentHealth agentHealth;

    private void Awake()
    {
        rightWeapon = GetComponentInChildren<Weapon>();
        agentHealth = GetComponent<AgentHealth>();
    }

    public void Attack(float duration, GuardDirection direction)
    {
        rightWeapon.EnterDamageState(duration, direction, agentHealth);
    }
}
