using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    [Header("Choice Buttons")]
    public GameObject choicePanel;     // A panel that holds all 3 buttons
    public Button leaveButton;
    public Button questButton;
    public Button shopButton;
    public GameObject shopBox;
    public GameObject questBox;
    private int index;
    [HideInInspector] public FPSCameraController cameraController;
    [HideInInspector] public FPSControllerWithStates playerController;

    void Start()
    {
        textComponent.text = string.Empty;
        choicePanel.SetActive(false);  // Hide buttons at start
        StartDialogue();

        // Assign button listeners
        leaveButton.onClick.AddListener(OnLeave);
        questButton.onClick.AddListener(OnQuest);
        shopButton.onClick.AddListener(OnShop);
    }

    void Update()
    {
        // Block mouse clicks from advancing dialogue when choices are showing
        if (choicePanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            // Last line reached — show choices instead of closing
            ShowChoices();
        }
    }

    void ShowChoices()
    {
        choicePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnLeave()
    {
        choicePanel.SetActive(false);
        gameObject.SetActive(false);
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

    public void OnQuest()
    {
        choicePanel.SetActive(false);
        gameObject.SetActive(false);
        questBox.SetActive(true);
        Debug.Log("Quest button pressed");
    }

    public void OnShop()
    {
        choicePanel.SetActive(false);
        gameObject.SetActive(false);
        shopBox.SetActive(true);
        Debug.Log("Shop button pressed");
    }
}