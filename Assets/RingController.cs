using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RingController : MonoBehaviour
{
    public PhotonView thisView;

    [PunRPC]
    public void DestroyRing()
    {
        if (thisView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
