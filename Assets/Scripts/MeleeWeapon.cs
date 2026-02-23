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

        // Simple debug attack (later: raycast / collider / animation trigger)
        Debug.Log("Melee swing! Damage: " + data.damage);

        // Here you would play animation, detect hits in cone/area, etc.
    }
}

