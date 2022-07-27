using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using QuantumTek.QuantumUI;

public class SettingsControl : MonoBehaviourPunCallbacks
{
    public QUI_OptionList gameTypeOptions;
    public QUI_OptionList fieldSetupOptions;
    public QUI_OptionList camSetupOptions;
    private PhotonView thisView;
    private MultiplayerSetting settings;
    private BasicMenu basic;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("Settings").GetComponent<MultiplayerSetting>();
        basic = GameObject.Find("Settings").GetComponent<BasicMenu>();
        thisView = GetComponent<PhotonView>();

        settings.resetSettings();
    }

    public void setGameTypeLeft()
    {
        if (!PhotonNetwork.IsConnected)
        {
            setGameTypeLeftHelper();
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
            return;
        thisView.RPC("setGameTypeLeftHelper", RpcTarget.AllBuffered);
    }

    public void setGameTypeRight()
    {
        if (!PhotonNetwork.IsConnected)
        {
            setGameTypeRightHelper();
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
            return;
        thisView.RPC("setGameTypeRightHelper", RpcTarget.AllBuffered);
    }

    public void setFieldSetupLeft()
    {
        if (!PhotonNetwork.IsConnected)
        {
            setFieldSetupLeftHelper();
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
            return;
        thisView.RPC("setFieldSetupLeftHelper", RpcTarget.AllBuffered);
    }

    public void setFieldSetupRight()
    {
        if(!PhotonNetwork.IsConnected)
        {
            setFieldSetupRightHelper();
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
            return;
        thisView.RPC("setFieldSetupRightHelper", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void setGameTypeLeftHelper()
    {
        settings.setGameTypeLeft();
        gameTypeOptions.ChangeOption(-1);
    }

    [PunRPC]
    public void setGameTypeRightHelper()
    {
        settings.setGameTypeRight();
        gameTypeOptions.ChangeOption(1);
    }

    [PunRPC]
    public void setFieldSetupLeftHelper()
    {
        settings.setFieldSetupLeft();
        fieldSetupOptions.ChangeOption(-1);
    }

    [PunRPC]
    public void setFieldSetupRightHelper()
    {
        settings.setFieldSetupRight();
        fieldSetupOptions.ChangeOption(1);
    }

    public void setCamSetupLeft()
    {
        settings.setCamSetupLeft();
        camSetupOptions.ChangeOption(-1);
    }

    public void setCamSetupRight()
    {
        settings.setCamSetupRight();
        camSetupOptions.ChangeOption(1);
    }

    public void ChangeToScene(int x)
    {
        basic.ChangeToScene(x);
    }

    public void joinGame()
    {
        PhotonRoom.room.joinGame();
    }

    public void DisconnectPhoton()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            FindObjectOfType<PhotonRoom>().OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ChangeToScene(0);
    }
}
