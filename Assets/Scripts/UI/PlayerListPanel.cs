using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListPanel : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private GameObject playerListPrefab;
    [SerializeField] private GameObject [] listItemRoots;
    [SerializeField] private int[] actorNumbers = new int[4];

    private PhotonRoom roomManager;

    private void Awake()
    {
        roomManager = FindObjectOfType<PhotonRoom>();

        //clear out actorNumbers array
        for(int i = 0; i<4; i++)
            actorNumbers[i] = -1;
    }

    public void AddNewPlayer(Player newPlayer)
    {
        int tempActorNumber = 0;
        int tempPos = 0;
        //Add new player to players array
        for (int i = 0; i < 4; i++)
        {
            if(actorNumbers[i] == -1)
            {
                actorNumbers[i] = newPlayer.ActorNumber;
                tempActorNumber = actorNumbers[i];
                tempPos = i;
                break;
            }
        }

        RaiseEventToRefreshList();        
    }

    private void OnClickMoveItemDown(int targetActorNumber)
    {
        for (int i = 0; i < 4; i++)
        {
            //find target player
            if (actorNumbers[i] == targetActorNumber)
            {
                //don't allow moving down further than spot 4
                if (i >= 3) { return; }

                //move player down the array
                if (actorNumbers[i + 1] == -1) //check if next spot is free
                {
                    actorNumbers[i + 1] = actorNumbers[i];
                    actorNumbers[i] = -1;
                }
                else //swap players
                {
                    int temp = actorNumbers[i + 1];
                    actorNumbers[i + 1] = actorNumbers[i];
                    actorNumbers[i] = temp;
                }

                break;
            }
        }

        RaiseEventToRefreshList();
    }

    private void RaiseEventToRefreshList()
    {
        if(!PhotonNetwork.IsMasterClient) { return; }
        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All

        };

        UpdatePlayerProperties();
        PhotonNetwork.RaiseEvent(0, (object)actorNumbers, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    private void UpdatePlayerProperties()
    {   
        //Add new player to players array
        for (int i = 0; i < 4; i++)
        {
            //make sure actornumber index isn't empty
            if (actorNumbers[i] == -1)
            {   
                continue;
            }
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                //find player by actornumber
                if (actorNumbers[i] == p.ActorNumber)
                {
                    roomManager.UpdatePlayerProperties(p, i);
                    break;
                }
            }
        }
    }

    private void OnClickMoveItemUp(int targetActorNumber)
    {
        for (int i = 0; i < 4; i++)
        {
            //find target player
            if (actorNumbers[i] == targetActorNumber)
            {
                //don't allow moving down further than spot 1
                if (i <= 0) { return; }

                //move player down the array
                if (actorNumbers[i - 1] == -1) //check if next upper spot is free
                {
                    actorNumbers[i - 1] = actorNumbers[i];
                    actorNumbers[i] = -1;
                }
                else //swap players
                {
                    int temp = actorNumbers[i - 1];
                    actorNumbers[i - 1] = actorNumbers[i];
                    actorNumbers[i] = temp;
                }

                break;
            }
        }

        RaiseEventToRefreshList();
    }

    public void RefreshPlayerList(int [] newActorNumbers)
    {
        actorNumbers = newActorNumbers;

        for(int i = 0; i < 4; i++)
        {
            //Remove existing player list item
            PlayerListItem listItem = listItemRoots[i].GetComponentInChildren<PlayerListItem>();
            if (listItem != null)
            {
                listItem.btn_down.onClick.RemoveAllListeners();
                listItem.btn_up.onClick.RemoveAllListeners();
                Destroy(listItem.gameObject);
            }

            //if the playerlist item exists, create one in that spot
            if (actorNumbers[i] != -1)
            {
                //Debug.Log("new item for: " + actorNumbers[i]);
                PlayerListItem newItem;
                
                newItem = Instantiate(playerListPrefab, listItemRoots[i].transform.position, Quaternion.identity).GetComponent<PlayerListItem>();

                //find player name
                foreach(Player p in PhotonNetwork.PlayerList)
                {
                    if(p.ActorNumber == actorNumbers[i])
                    {
                        newItem.Setup(p.NickName);
                    }
                }

                newItem.gameObject.transform.parent = listItemRoots[i].transform;
                newItem.gameObject.transform.localScale = Vector3.one;
                int num = actorNumbers[i];
                newItem.btn_down.onClick.AddListener(() => OnClickMoveItemDown(num));
                newItem.btn_up.onClick.AddListener(() => OnClickMoveItemUp(num));
            }
        }
    }

    #region - PUN Methods -
    [PunRPC]
    public void RemovePlayer(int targetActorNumber)
    {
        //Remove target player from players array
        for (int i = 0; i < 4; i++)
        {
            if (actorNumbers[i] == targetActorNumber)
            {
                actorNumbers[i] = -1;
                break;
            }
        }

        //Remove player list item from ui
        RaiseEventToRefreshList();
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)0)
        {
            if ((int[])photonEvent.CustomData != null)
            {
                //Debug.Log("refresh player list called: " + (int[])photonEvent.CustomData);

                RefreshPlayerList((int[])photonEvent.CustomData);

            }
        }
    }
    #endregion

}


