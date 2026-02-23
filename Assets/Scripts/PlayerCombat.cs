using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviourPunCallbacks
{
    private PlayerInventory inventory;

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            WeaponBase weapon = inventory.GetActiveWeapon();
            if (weapon != null)
            {
                weapon.PerformAttack();
                Debug.Log(weapon);
            }
            else
            {
                Debug.Log("No Weapon In hand");
            }
        }
    }
}