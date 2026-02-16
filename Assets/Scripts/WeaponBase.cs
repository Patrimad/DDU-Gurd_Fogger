using UnityEngine;
using Photon.Pun;

public abstract class WeaponBase : MonoBehaviourPunCallbacks
{
    public WeaponData data;

    protected float currentCooldownTimer = 0f;

    public bool IsReady
    {
        get
        {
            if (currentCooldownTimer <= 0f)
            {
                return true;
            }
            return false;
        }
    }

    public virtual void Initialize(WeaponData weaponData)
    {
        data = weaponData;
        currentCooldownTimer = 0f;
    }

    public virtual void OnEquip(PlayerInventory owner)
    {
        // Pamientaj przepisac
    }

    public virtual void OnUnequip()
    {
        // Pamientaj przepisac
    }

    public abstract void PerformAttack();

    protected void Update()
    {
        if (currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= Time.deltaTime;
        }
    }
}