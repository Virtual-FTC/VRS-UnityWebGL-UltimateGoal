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
        print("destroying ring");
        if (thisView.IsMine)
        {
            print("view is mine");
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
