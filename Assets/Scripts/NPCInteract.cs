using Unity.AppUI.UI;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public Dialogue dialogue;
    public FPSCameraController cameraController;
    public FPSControllerWithStates playerController;
    private bool playerInRange;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            dialogue.cameraController = cameraController;
            dialogue.playerController = playerController;
            dialogue.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraController.enabled = false;
            playerController.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            cameraController = other.GetComponentInChildren<FPSCameraController>();
            playerController = other.GetComponent<FPSControllerWithStates>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}