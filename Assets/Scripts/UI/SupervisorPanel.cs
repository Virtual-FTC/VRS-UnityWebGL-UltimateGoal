using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SupervisorPanel : MonoBehaviour
{
    private readonly string pass = "1234";

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button submitButton, closeButton;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        submitButton.onClick.AddListener(OnPasswordEntered);
        closeButton.onClick.AddListener(HidePanel);
    }

    public void ShowPanel()
    {
        panelRoot.SetActive(true);
    }

    public void HidePanel()
    {
        panelRoot.SetActive(false);
    }

    private void OnPasswordEntered()
    {
        if(inputField.text == pass)
        {
            SetupSupervisor();
        }
        else
        {
            feedbackText.text = "Password Failed. Try Again.";
        }
    }

    private void SetupSupervisor()
    {
        Debug.LogWarning("setup supervisor not implemented yet");

        feedbackText.text = "Password Accepted";
    }
}
