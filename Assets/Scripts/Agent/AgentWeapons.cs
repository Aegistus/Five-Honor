using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentWeapons : MonoBehaviour
{
    [SerializeField] Weapon rightWeapon;

    public Weapon RightWeapon => rightWeapon;

    private void Awake()
    {
        rightWeapon = GetComponentInChildren<Weapon>();
    }

    public void Attack(float duration, GuardDirection direction)
    {
        print("Attacking");
        rightWeapon.EnterDamageState(duration, direction);
    }
}
