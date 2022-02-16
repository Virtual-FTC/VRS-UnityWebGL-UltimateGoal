using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RedWobble : MonoBehaviour
{
    [PunRPC]
    public void swapPhotonViews(bool toEnable)
    {
        GetComponent<PhotonRigidbodyView>().enabled = toEnable;
        GetComponent<PhotonTransformView>().enabled = !toEnable;
    }
}
