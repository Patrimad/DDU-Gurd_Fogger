using UnityEngine;

public abstract class WeaponFactory
{
    public abstract WeaponData CreateWeaponData(SOWeaponTemplate template, int questLevel);
}

public class BalancedRogueLikeFactory : WeaponFactory
{
    public override WeaponData CreateWeaponData(SOWeaponTemplate template, int questLevel)
    {
        float levelMultiplier = 1f + (questLevel * 0.15f);
        float rngDamage = template.baseDamage * levelMultiplier * Random.Range(0.85f, 1.15f);
        float rngCooldown = template.baseAttackCooldown * Random.Range(0.9f, 1.1f);

        WeaponData data = new WeaponData(
            template.templateId,
            template.family,
            template.type,
            rngDamage,
            rngCooldown,
            template.baseRange,
            (questLevel / 5) + 1
        );

        return data;
    }
}