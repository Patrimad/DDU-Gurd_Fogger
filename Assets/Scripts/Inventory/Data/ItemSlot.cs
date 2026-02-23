using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public WeaponData weaponData = new WeaponData();

    public bool IsEmpty()
    {
        if (weaponData == null)
        {
            return true;
        }
        return weaponData.IsEmpty();
    }

    public void Clear()
    {
        weaponData = new WeaponData();
    }
}