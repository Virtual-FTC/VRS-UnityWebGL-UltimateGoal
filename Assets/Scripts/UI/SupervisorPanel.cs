using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class SupervisorPanel : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private readonly string pass = "1234";

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button logOnButton, logOffButton, closeButton;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_InputField inputField;

    Player[] photonPlayers;

    private void Awake()
    {
        logOnButton.onClick.AddListener(OnPasswordEntered);
        logOffButton.onClick.AddListener(LogOff);
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

    private void LogOff()
    {
        UserSingleton.instance.localUserType = User.student;
        feedbackText.text = UserSingleton.instance?.localUserType.ToString();
    }

    private void OnPasswordEntered()
    {
        if(inputField.text == pass)
        {
            SetupSupervisor();
        }
        else
        {
            LogOff();
            feedbackText.text = "Password Failed. Try Again. " + UserSingleton.instance?.localUserType.ToString();
        }
    }

    private void SetupSupervisor()
    {
        Debug.LogWarning("setup supervisor not implemented yet");
        UserSingleton.instance.localUserType = User.supervisor;
        feedbackText.text = "Password Accepted " + UserSingleton.instance?.localUserType.ToString();
    }

    public void SwitchMasterClient()
    {
        photonPlayers = PhotonNetwork.PlayerList;

        //Delay Start
        foreach(Player p in photonPlayers)
        {
            if(p != PhotonNetwork.LocalPlayer)
            {
                PhotonNetwork.SetMasterClient(p);
                return;
            }
        }
    }
}
