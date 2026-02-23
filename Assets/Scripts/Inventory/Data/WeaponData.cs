using System;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public string templateId = "";
    public string family = "";
    public WeaponType type = WeaponType.None;
    public float damage = 0f;
    public float attackCooldown = 0f;
    public float range = 0f;
    public int tier = 0;

    public WeaponData()
    {
    }

    public WeaponData(string templateId, string family, WeaponType type, float damage, float attackCooldown, float range, int tier)
    {
        this.templateId = templateId;
        this.family = family;
        this.type = type;
        this.damage = damage;
        this.attackCooldown = attackCooldown;
        this.range = range;
        this.tier = tier;
    }

    public bool IsEmpty()
    {
        if (string.IsNullOrEmpty(templateId))
        {
            return true;
        }
        return false;
    }
}