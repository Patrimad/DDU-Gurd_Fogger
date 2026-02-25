using UnityEngine;

public class BackButtonDialogue : MonoBehaviour
{
    public GameObject shopBox;
    public GameObject choicePanel;
    public GameObject questBox;
    public GameObject dialogueBox;

    public void OnClose()
    {
        shopBox.SetActive(false);
        questBox.SetActive(false);
        dialogueBox.SetActive(true);
        choicePanel.SetActive(true);  // goes back to dialogue
    }
}