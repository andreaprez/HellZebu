using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public bool empty = true;
    public bool active = false;

    bool weaponOnSlot;
    public Weapon currentWeapon;
    private void Start()
    {
        weaponOnSlot = false;
    }
    private void Update()
    {
        if (GetComponentInChildren<Weapon>() != null&&weaponOnSlot==false)
        {
            weaponOnSlot = true;
            currentWeapon = GetComponentInChildren<Weapon>();
        }
    }
}
