using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Container;
    private FPSCameraController cameraController;
    private FPSControllerWithStates playerController;

    void Start()
    {
        // Find the local player's components
        FPSControllerWithStates[] allPlayers = FindObjectsOfType<FPSControllerWithStates>();
        foreach (FPSControllerWithStates player in allPlayers)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                playerController = player;
                cameraController = player.GetComponentInChildren<FPSCameraController>();
                break;
            }
        }
    }

    void Update()
    {
        // Keep trying to find player until found
        if (playerController == null)
        {
            FPSControllerWithStates[] allPlayers = FindObjectsOfType<FPSControllerWithStates>();
            foreach (FPSControllerWithStates player in allPlayers)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    playerController = player;
                    cameraController = player.GetComponentInChildren<FPSCameraController>();
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Container.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraController.enabled = false;
            playerController.enabled = false;
        }
    }

    public void ResumeButton()
    { 
        Container.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraController != null)
        {
            cameraController.enabled = true;
            Debug.Log("Camera re-enabled");
        }
        else
            Debug.Log("cameraController is NULL!");

        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player re-enabled");
        }
        else
            Debug.Log("playerController is NULL!");
    }

    public void MainMenuButton()
    {
        PhotonNetwork.LeaveRoom();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Multiplayer Scene");
    }
}
