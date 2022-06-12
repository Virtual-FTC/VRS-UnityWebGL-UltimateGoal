using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonStats : MonoBehaviourPunCallbacks
{
    public static PhotonStats instance;
    public Text log;
    private string log_roomList = "";
    NetworkManager netman;
    public Server region = Server.us;
    public Dropdown serverChooser;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);

        UpdateServerLocation(serverChooser);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            log.text = $"Chosen Server: {region}\n" +
                $"Server Info : {PhotonNetwork.PhotonServerSettings} \n" +
            $"<color=green>Region: {PhotonNetwork.CloudRegion}</color>\n" +
            $"{PhotonNetwork.CurrentLobby} | Player Count: {PhotonNetwork.CountOfPlayers}\n";

            if (PhotonNetwork.CurrentRoom != null)
                log.text += $"Room: {PhotonNetwork.CurrentRoom.ToStringFull()}\n";
        }
        else
            log.text = $"Photon disconnected. Chosen Region: {region}";

        if(Input.GetKeyDown(KeyCode.U))
        {
            if(region == Server.us)
            {
                region = Server.usw;
            }
            else
            {
                region = Server.us;
            }
            log.text += $"Region switched to {region}";
        }
    }

    public void UpdateServerLocation(Dropdown newLoc)
    {
        switch(newLoc.captionText.text)
        {
            case "Server: US East": region = Server.us; break;
            case "Server: US West": region = Server.usw; break;
            case "Server: Canada East": region = Server.cae; break;
            default: region = Server.usw; break;
        }
        Debug.Log($"Region switched to {region}");
    }
}

public enum Server
{
    us,usw,cae
}
