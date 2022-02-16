using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SettingsControl : MonoBehaviour
{
    private MultiplayerSetting settings;
    private BasicMenu basic;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("Settings").GetComponent<MultiplayerSetting>();
        basic = GameObject.Find("Settings").GetComponent<BasicMenu>();

        settings.resetSettings();
    }

    public void setGameTypeLeft()
    {
        if (PhotonNetwork.IsMasterClient)
            settings.setGameTypeLeft();
    }

    public void setGameTypeRight()
    {
        if (PhotonNetwork.IsMasterClient)
            settings.setGameTypeRight();
    }

    public void setFieldSetupLeft()
    {
        if (PhotonNetwork.IsMasterClient)
            settings.setFieldSetupLeft();
    }

    public void setFieldSetupRight()
    {
        if (PhotonNetwork.IsMasterClient)
            settings.setFieldSetupRight();
    }

    public void setCamSetupLeft()
    {
        settings.setCamSetupLeft();
    }

    public void setCamSetupRight()
    {
        settings.setCamSetupRight();
    }

    public void ChangeToScene(int x)
    {
        basic.ChangeToScene(x);
    }

    public void joinGame()
    {
        PhotonRoom.room.joinGame();
    }
}
