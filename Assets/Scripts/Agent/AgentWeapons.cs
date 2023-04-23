using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentWeapons : MonoBehaviour
{
    [SerializeField] Weapon leftWeapon;
    [SerializeField] Weapon rightWeapon;

    public Weapon LeftWeapon => leftWeapon;
    public Weapon RightWeapon => rightWeapon;


}
