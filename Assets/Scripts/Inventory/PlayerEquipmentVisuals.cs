using UnityEngine;

public class PlayerEquipmentVisuals : MonoBehaviour
{
    public Transform rightHandBone;

    private GameObject currentModel;

    public void AttachWeaponModel(GameObject weaponInstance)
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        currentModel = weaponInstance;
        currentModel.transform.SetParent(rightHandBone);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;
    }

    public void RemoveCurrentModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
        }
    }
}