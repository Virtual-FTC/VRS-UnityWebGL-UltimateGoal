using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string playerName;

    // Join Field
    private bool lobbyIsActive;

    // Create Field
    private string fieldName;
    private int maxPlayers = 1;
    public string region = "us";

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
       if (!PhotonNetwork.IsConnected)
       {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = ServerSelector.instance?.GetRegion();
            
            PhotonNetwork.ConnectUsingSettings();
       }
    }

    // Callbacks
    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.CloudRegion != ServerSelector.instance?.GetRegion() + "/*")
            Debug.Log("failed to connecto correct server");

        Debug.Log($"Connected to master server: {PhotonNetwork.CloudRegion}");
        PhotonNetwork.JoinLobby();
         
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to lobby");
        lobbyIsActive = true;
    }

    

    /*
    // Photon Methods
    public override void OnConnected()
    {
        // 1
        base.OnConnected();
        // 2
        connectionStatus.text = "Connected to Photon!";
        connectionStatus.color = Color.green;
        roomJoinUI.SetActive(true);
        buttonLoadArena.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 3
        isConnecting = false;
        controlPanel.SetActive(true);
        Debug.LogError("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        // 4
        if (PhotonNetwork.IsMasterClient)
        {
            buttonLoadArena.SetActive(true);
            buttonJoinRoom.SetActive(false);
            playerStatus.text = "You are Lobby Leader";
        }
        else
        {
            playerStatus.text = "Connected to Lobby";
        }
    }

    */

    // Setting var
    public void setPlayerName(string name)
    {
        playerName = name;
    }

    public void setFieldName(string name)
    {
        fieldName = name;
    }

    public void setMaxPlayers(int num)
    {
        maxPlayers += num;
        if (maxPlayers > 4)
            maxPlayers = 1;
        else if (maxPlayers < 1)
            maxPlayers = 4;
        Debug.Log("maxPlayers: " + maxPlayers);
    }

    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!lobbyIsActive)
                return;
            PhotonNetwork.LocalPlayer.NickName = playerName; //1
            Debug.Log(PhotonNetwork.LocalPlayer.NickName);
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)maxPlayers;
            options.IsVisible = true;
            options.EmptyRoomTtl = 0;
            PhotonNetwork.JoinOrCreateRoom(fieldName, options, TypedLobby.Default);
        }
    }

    public void JoinRoom(int index)
    {
        if (PhotonNetwork.IsConnected)
        {
            lobbyIsActive = true;
            PhotonNetwork.LocalPlayer.NickName = playerName; //1
            //PhotonNetwork.JoinRoom(rooms[index].Name);
            Debug.Log("Trying to join room");
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }        
    }

    public void ChangeScene(int sceneInx)
    {
        if (!lobbyIsActive)
            return;
        PhotonNetwork.LoadLevel(sceneInx);
    }
}