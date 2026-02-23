using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Container;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Container.SetActive(true);
        }
    }

    public void ResumeButton()
    { 
        Container.SetActive(false);
    }

    public void MainMenuButton()
    {
        PhotonNetwork.LeaveRoom();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Multiplayer Scene");
    }
}
