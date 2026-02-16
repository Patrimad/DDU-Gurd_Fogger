using UnityEngine;

[CreateAssetMenu(fileName = "SOWeaponTemplate", menuName = "ScriptableObjects/Weapons")]
public class SOWeaponTemplate : ScriptableObject
{
    public string templateId = "DefaultSword";
    public string family = "Sword";
    public WeaponType type = WeaponType.MeleeFast;

    public GameObject weaponModelPrefab;

    public float baseDamage = 10f;
    public float baseAttackCooldown = 1.2f;
    public float baseRange = 2.5f;

    public WeaponData GenerateData(int questLevel)
    {
        BalancedRogueLikeFactory factory = new BalancedRogueLikeFactory();
        return factory.CreateWeaponData(this, questLevel);
    }
}
