using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ServerSelector : MonoBehaviourPunCallbacks
{
    public static ServerSelector instance;
    
    public bool showDebugUI = false;
    public Text log;
    public TMPro.TMP_Dropdown dropdownMenu;
    public GameObject root_debugUI, root_menu;

    private string lastLog = "";
    public static readonly string SERVER_PREFS = "ServerPrefs"; //Player prefs key
    private string currentRegion = "";
    private bool showMenu = true;

    /// <summary>
    /// List of Servers for Players to choose from. Key is Regular Server Name and Value is the PUN Server Token
    /// </summary>
    private Dictionary<string, string> serverList = new Dictionary<string, string>()
    {
        //Server Name, Server Token
        { "US East", "us" },
        { "US West", "usw" },
        { "Canada East", "cae" }
    };

    void Start()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);

        LoadServerPrefs();
        CreateDropDownMenu();
        SetDropdownSelectionTo(currentRegion);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Collect Server Info
        if (PhotonNetwork.IsConnected)
        {
            LogMessage($"Chosen Server: {currentRegion}\n" +
                $"Server Info : {PhotonNetwork.PhotonServerSettings} \n" +
            $"<color=green>Region: {PhotonNetwork.CloudRegion}</color>\n" +
            $"{PhotonNetwork.CurrentLobby} | Player Count: {PhotonNetwork.CountOfPlayers}\n", clearScreen: true);

            if (PhotonNetwork.CurrentRoom != null)
                LogMessage($"Room: {PhotonNetwork.CurrentRoom.ToStringFull()}\n");
        }
        else
            LogMessage($"Photon disconnected. Chosen Region: {currentRegion}", clearScreen: true);

        ShowUI(SceneManager.GetActiveScene().buildIndex == 0);
    }

    public void ShowUI(bool show)
    {
        if(showMenu != show)
        {
            root_debugUI.SetActive(showDebugUI);
            root_menu.SetActive(show);
            showMenu = show;
        }
    }

    public string GetRegion()
    {
        return FindRegionToken(currentRegion);
    }

    /// <summary>
    /// Checks if player has previously selected a server before. Sets region to that choice if it exists.
    /// </summary>
    private void LoadServerPrefs()
    {
        if(PlayerPrefs.HasKey(SERVER_PREFS))
        {
            currentRegion = PlayerPrefs.GetString(SERVER_PREFS);
            if(!serverList.ContainsKey(currentRegion))
            {
                currentRegion = "US East"; //Default Server Region
                PlayerPrefs.SetString(SERVER_PREFS, currentRegion);
            }
        }
        else
        {
            currentRegion = "US East"; //Default Server Region
            PlayerPrefs.SetString(SERVER_PREFS, currentRegion);
        }
        SavePlayerPrefs();        
    }

    private void SavePlayerPrefs()
    {
        LogMessage($"Saving to player prefs region: {currentRegion}");
        PlayerPrefs.SetString(SERVER_PREFS, currentRegion);
        PlayerPrefs.Save();
    }

    private void SetDropdownSelectionTo(string selectedServer)
    {
        for(int i = 0; i < dropdownMenu.options.Count; i++)
        {
            if (dropdownMenu.options[i].text == selectedServer)
                dropdownMenu.value = i;
        }
    }

    /// <summary>
    /// Creates the Server Selection drop menu options with the list of servers from the serverList dictionary
    /// </summary>
    private void CreateDropDownMenu()
    {
        dropdownMenu.options.Clear();

        foreach (KeyValuePair<string, string> entry in serverList)
        {
            dropdownMenu.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = entry.Key });
        }

        int TempInt = dropdownMenu.value;
        dropdownMenu.value = dropdownMenu.value + 1;
        dropdownMenu.value = TempInt;

        dropdownMenu.onValueChanged.AddListener(delegate { UpdateServerLocation(dropdownMenu); });
    }

    /// <summary>
    /// Checks Server enums for given string and returns it. Will Return Server.usw if no match is found.
    /// </summary>
    /// <param name="newRegion"></param>
    /// <returns></returns>
    private string FindRegionToken(string newRegion)
    {
        if (serverList.ContainsKey(newRegion))
            return serverList[newRegion];
        else
            return "us";
    }

    /// <summary>
    /// Updates the Server region PUN will connect to when the Server Selector Dropmenu changes.
    /// </summary>
    /// <param name="newLoc"></param>
    public void UpdateServerLocation(TMPro.TMP_Dropdown newLoc)
    {
        currentRegion = newLoc.captionText.text;
        //LogMessage($"Region switched to {currentRegion}");
        SavePlayerPrefs();
    }

    /// <summary>
    /// Logs a message to the onscreen debug window (if enabled) and Debug.Log
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="clearScreen"></param>
    private void LogMessage(string msg, bool clearScreen = false)
    {
        if(showDebugUI)
        {
            if (clearScreen)
                log.text = msg;
            else
                log.text += msg;
        }
        if(lastLog != msg)
        {
            lastLog = msg;
            Debug.Log(msg);
        }

    }
}






