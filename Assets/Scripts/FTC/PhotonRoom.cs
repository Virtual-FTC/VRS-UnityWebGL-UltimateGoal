using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    private ExitGames.Client.Photon.Hashtable _customProperties = new ExitGames.Client.Photon.Hashtable();

    // Room info
    public static PhotonRoom room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;
    public int multiplayScene;
    private PlayerListPanel playerListPanel;

    // Player Info
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playersInGame;

    public GameObject[] playerUI;
    public GameObject[] spawnPositions;
    public CustomPlayer[] customPlayers = new CustomPlayer[4];
    public GameObject positionGam;

    // Delayed start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart;


    // Start is called before the first frame update
    void Awake()
    {
        playerListPanel = FindObjectOfType<PlayerListPanel>();

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Destroyed PhotonRoom");
            Destroy(transform.gameObject);
        }

        // Setup singleton (error over photonview is ok)
        if(PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if(PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(positionGam);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == multiplayScene)
        {
            isGameLoaded = true;

            //Delay
            if (MultiplayerSetting.multiplayerSetting.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void CreatePlayer()
    {
        if (UserSingleton.instance?.localUserType == User.supervisor)
            return;

        
        int spawnPos = (int)PhotonNetwork.LocalPlayer.CustomProperties[PLAYERPROPS.PLAYER_POS];

        var robot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), spawnPositions[spawnPos].transform.position, spawnPositions[spawnPos].transform.rotation, 0);
        robot.GetComponent<RobotController>().setStartPosition(spawnPositions[spawnPos].transform);
        
        Destroy(transform.gameObject);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("You are now in a room" + PhotonNetwork.CurrentRoom.ToString());
        
        GameObject roomLabel = GameObject.FindGameObjectWithTag("roomlabel");
        if(roomLabel != null)
        {
            try
            {
                roomLabel.GetComponent<TextMeshProUGUI>().text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            }
            catch{}
            
        }
        checkPlayers();
        SetPlayerType();

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                //skip supervisor
                if (UserSingleton.instance?.localUserType == User.supervisor)
                {
                    if(p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        continue;
                    }
                }
                playerListPanel.AddNewPlayer(p);
            }
        }
    }


    private void SetPlayerType()
    {
        if (UserSingleton.instance?.localUserType == User.supervisor)
        {
            _customProperties[PLAYERPROPS.PLAYER_TYPE] = (int)User.supervisor;
        }
        else
        {
            _customProperties[PLAYERPROPS.PLAYER_TYPE] = (int)User.student;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
    }

    public void UpdatePlayerProperties(Player targetPlayer, int pos)
    {
        Team newTeam = pos <= 1 ? Team.blue : Team.red;
        Debug.Log($"Updating player custom properties: {targetPlayer.NickName} pos = {pos}, team = {newTeam}");
        
        ExitGames.Client.Photon.Hashtable _customProperties = new ExitGames.Client.Photon.Hashtable();
        _customProperties[PLAYERPROPS.PLAYER_POS] = pos;
        _customProperties[PLAYERPROPS.PLAYER_TEAM] = newTeam;
        targetPlayer.SetCustomProperties(_customProperties);
        PhotonNetwork.CurrentRoom.SetCustomProperties(_customProperties);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        checkPlayers();
        addNewPlayerToUI(newPlayer);        
    }

    private void checkPlayers()
    {
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;

        //Delay Start
        if (MultiplayerSetting.multiplayerSetting.delayStart)
        {
            Debug.Log("Players in room " + playersInRoom);
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                readyToStart = false;
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
        }
        else
        {
            StartGame();
        }
    }

    private void updatePlayerUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int x = 0; x < customPlayers.Length; x++)
            {
                if (customPlayers[x] != null)
                {
                    playerUI[x].GetComponent<Text>().text = customPlayers[x].player.NickName;
                    playerUI[x].SetActive(true);
                    return;
                }
                else
                    playerUI[x].SetActive(false);
            }
        }
    }

    public void updateTeam(int val)
    {
        
    }

    public void updatePos(int val)
    {

    }

    private void Update()
    {
        photonPlayers = PhotonNetwork.PlayerList;

        foreach(Player p in photonPlayers)
        {
            if (p.CustomProperties.ContainsKey(PLAYERPROPS.PLAYER_TYPE))
            {
                if ((int)p.CustomProperties[PLAYERPROPS.PLAYER_TYPE] == (int)User.supervisor)
                {
                    //removeNewPlayerFromUI(p);
                    return;
                }
            }
        }
    }

    private void addNewPlayerToUI(Player p)
    {
        //if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log(customPlayers.Length);
            for (int x = 0; x < customPlayers.Length; x++)
            {
                if (customPlayers[x] == null)
                {
                    customPlayers[x] = new CustomPlayer();
                    if (x < 2)
                    {
                        customPlayers[x].team = 0;
                        customPlayers[x].pos = x;
                    }
                    else
                    {
                        customPlayers[x].team = 1;
                        customPlayers[x].pos = x - 2;
                    }
                    customPlayers[x].player = p;
                    Debug.Log("New player named : " + customPlayers[x].player.NickName);
                    playerListPanel.AddNewPlayer(p);
                    return;
                }
            }
        }
    }

    private void removeNewPlayerFromUI(Player p)
    {
        for (int x = 0; x < customPlayers.Length; x++)
        {
            if (customPlayers[x] == null)
                continue;
            if (customPlayers[x].player.ActorNumber == p.ActorNumber)
            {
                customPlayers[x] = null;                
                playerListPanel.RemovePlayer(p.ActorNumber);
                break;
            }
        }

        checkPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        photonPlayers = PhotonNetwork.PlayerList;
            
        removeNewPlayerFromUI(otherPlayer);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
       
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }

    public void joinGame()
    {
        //Debug.Log("I trying to join game");
        if (PhotonNetwork.IsConnected)
        {
            if (readyToStart)
            {
                StartGame();
            }
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }

    void StartGame()
    {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (MultiplayerSetting.multiplayerSetting.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(multiplayScene);

    }

    
}

public class CustomPlayer
{
    public int team; // Tells me team as well as UI slot
    public int pos; // Tells me position as well as UI slot
    public Player player = null; // Tells me name
}

public enum Team
{
    blue, red
}
