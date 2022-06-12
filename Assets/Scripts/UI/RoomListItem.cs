using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    public Text label;
    public Button joinBtn;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        label.text = roomInfo.Name;        
    }
}
