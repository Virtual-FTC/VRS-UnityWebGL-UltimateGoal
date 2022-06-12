using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject prefab_RoomListItem;
    public Transform roomContentRoot;
    private Dictionary<string, GameObject> cachedRoomList = new Dictionary<string, GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                Debug.Log($"Room {info.Name} removed");
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    Destroy(cachedRoomList[info.Name]);
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                GameObject newRoom = Instantiate(prefab_RoomListItem, roomContentRoot);
                RoomListItem newRoomListItem = newRoom.GetComponentInChildren<RoomListItem>();
                newRoomListItem.SetRoomInfo(info);
                newRoomListItem.joinBtn.onClick.AddListener(() => OnJoinRoomButtonClicked(info.Name));
                cachedRoomList.Add(info.Name, newRoom);
            }
        }
    }

    private void OnJoinRoomButtonClicked(string _roomName)
    {
        //if you are already in a lobby, you need to leave lobby

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(_roomName);
    }
}
