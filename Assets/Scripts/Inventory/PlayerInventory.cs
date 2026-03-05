using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class PlayerInventory : MonoBehaviourPunCallbacks
{
    public ItemSlot[] slots = new ItemSlot[4];

    public PlayerEquipmentVisuals equipmentVisuals;

    private int activeSlotIndex = 0;
    private WeaponBase currentlyEquippedWeapon;

    private const string PROP_INVENTORY = "InvData";
    private const string PROP_ACTIVE_SLOT = "ActiveSlotIdx";

    private string inventoryJsonCache;

    void Start()
    {
        InitializeInventory();

        if (PhotonNetwork.IsMasterClient)
        {
            // Master gives starting weapons to everyone
            photonView.RPC(nameof(RPC_GiveStartingWeapons), RpcTarget.All);
        }

        SyncInventoryToNetwork();
    }

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        HandleSlotInput();
    }

    private void InitializeInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            slots[i].Clear();
        }
    }

    private void HandleSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryChangeActiveSlot(0);
            Debug.Log("Item slot 1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryChangeActiveSlot(1);
            Debug.Log("Item slot 2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TryChangeActiveSlot(2);
            Debug.Log("Item slot 3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TryChangeActiveSlot(3);
            Debug.Log("Item slot 4");
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.01f)
        {
            int next = activeSlotIndex + 1;
            if (next >= 4)
            {
                next = 0;
            }
            TryChangeActiveSlot(next);
            Debug.Log($"{next} - scroll");
        }
        else if (scroll < -0.01f)
        {
            int prev = activeSlotIndex - 1;
            if (prev < 0)
            {
                prev = 3;
            }
            TryChangeActiveSlot(prev);
            Debug.Log($"{prev} - scroll");
        }
    }

    private void TryChangeActiveSlot(int newIndex)
    {
        if (newIndex < 0 || newIndex >= 4)
        {
            return;
        }
        if (newIndex == activeSlotIndex)
        {
            return;
        }

        UnequipCurrent();
        activeSlotIndex = newIndex;
        EquipFromCurrentSlot();

        photonView.RPC(nameof(NetworkSyncActiveSlot), RpcTarget.OthersBuffered, activeSlotIndex);

        Hashtable props = new Hashtable();
        props[PROP_ACTIVE_SLOT] = activeSlotIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void UnequipCurrent()
    {
        if (currentlyEquippedWeapon != null)
        {
            currentlyEquippedWeapon.OnUnequip();
            if (equipmentVisuals != null)
            {
                equipmentVisuals.RemoveCurrentModel();
            }
            Destroy(currentlyEquippedWeapon.gameObject);
            currentlyEquippedWeapon = null;
        }
    }

    private void EquipFromCurrentSlot()
    {
        ItemSlot slot = slots[activeSlotIndex];
        if (slot.IsEmpty())
        {
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("Weapons/" + slot.weaponData.templateId);
        if (prefab == null)
        {
            Debug.LogWarning("Weapon prefab not found: " + slot.weaponData.templateId);
            return;
        }

        GameObject instance = Instantiate(prefab, transform);
        WeaponBase weapon = instance.GetComponent<WeaponBase>();
        if (weapon == null)
        {
            Debug.LogError("Weapon prefab missing WeaponBase component!");
            Destroy(instance);
            return;
        }

        weapon.Initialize(slot.weaponData);
        weapon.OnEquip(this);

        if (equipmentVisuals != null)
        {
            equipmentVisuals.AttachWeaponModel(instance);
        }

        currentlyEquippedWeapon = weapon;
    }

    public WeaponBase GetActiveWeapon()
    {
        return currentlyEquippedWeapon;
    }

    public bool TryAddWeapon(WeaponData newData)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty())
            {
                slots[i].weaponData = newData;
                SyncInventoryToNetwork();
                return true;
            }
        }
        return false;
    }

    private void SyncInventoryToNetwork()
    {
        SerializableInventory wrapper = new SerializableInventory();
        wrapper.slots = slots;

        inventoryJsonCache = JsonUtility.ToJson(wrapper);
        Hashtable props = new Hashtable();
        props[PROP_INVENTORY] = inventoryJsonCache;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer != photonView.Owner)
        {
            return;
        }

        if (changedProps.ContainsKey(PROP_ACTIVE_SLOT))
        {
            activeSlotIndex = (int)changedProps[PROP_ACTIVE_SLOT];
            UnequipCurrent();
            EquipFromCurrentSlot();
        }

        if (changedProps.ContainsKey(PROP_INVENTORY))
        {
            string json = (string)changedProps[PROP_INVENTORY];
            if (json != inventoryJsonCache)
            {
                inventoryJsonCache = json;
                SerializableInventory loaded = JsonUtility.FromJson<SerializableInventory>(json);
                if (loaded != null && loaded.slots != null && loaded.slots.Length == 4)
                {
                    slots = loaded.slots;
                }
                UnequipCurrent();
                EquipFromCurrentSlot();
            }
        }
    }

    [PunRPC]
    private void NetworkSyncActiveSlot(int index)
    {
        activeSlotIndex = index;
        UnequipCurrent();
        EquipFromCurrentSlot();
    }
    [PunRPC]
    private void RPC_GiveStartingWeapons()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Clear();
        }

        SOWeaponTemplate swordSO = Resources.Load<SOWeaponTemplate>("WeaponTemplates/Sword_Start");
        if (swordSO != null)
        {
            WeaponData sword = swordSO.GenerateData(1);
            TryAddWeapon(sword);
        }
        else
        {
            Debug.LogError("Master: Could not load Sword_Fast_01 template!");
        }

        // Force equip slot 0
        activeSlotIndex = 0;
        EquipFromCurrentSlot();

        // Sync everything
        photonView.RPC(nameof(NetworkSyncActiveSlot), RpcTarget.OthersBuffered, activeSlotIndex);
        SyncInventoryToNetwork();
    }
}

[System.Serializable]
public class SerializableInventory
{
    public ItemSlot[] slots;
}