using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonStats : MonoBehaviourPunCallbacks
{
    public Text log;
    private string log_roomList = "";
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsConnected)
        {
            log.text = $"Server Info : {PhotonNetwork.PhotonServerSettings} \n" +
            $"<color=green>Region: {PhotonNetwork.CloudRegion}</color>\n" +
            $"{PhotonNetwork.CurrentLobby} | Player Count: {PhotonNetwork.CountOfPlayers}\n";

            if (PhotonNetwork.CurrentRoom != null)
                log.text += $"Room: {PhotonNetwork.CurrentRoom.ToStringFull()}\n";
        }
    }
}
