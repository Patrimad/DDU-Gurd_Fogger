using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    public override void PerformAttack()
    {
        if (!IsReady)
        {
            return;
        }

        currentCooldownTimer = data.attackCooldown;
        Debug.Log("Melee swing! Damage: " + data.damage);
    }
}

