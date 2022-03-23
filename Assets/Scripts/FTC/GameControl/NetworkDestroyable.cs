using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkDestroyable : MonoBehaviourPun
{
    [PunRPC]
    public void NetworkDestroy()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(base.photonView);
    }
}
