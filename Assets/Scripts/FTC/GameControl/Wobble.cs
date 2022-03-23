using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wobble : MonoBehaviourPun
{
    [PunRPC]
    public void NetworkGrab(PhotonMessageInfo info)
    {
        this.transform.SetParent(info.photonView.gameObject.GetComponent<GrabberControl>().robot);
        this.transform.localPosition = new Vector3(0f, -0.39f, 0.3f);
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<PhotonRigidbodyView>().enabled = false;
        this.GetComponent<PhotonTransformView>().enabled = true;
    }

    [PunRPC]
    public void NetworkStopGrab(PhotonMessageInfo info)
    {
        this.transform.SetParent(null);
        this.GetComponent<PhotonRigidbodyView>().enabled = true;
        this.GetComponent<PhotonTransformView>().enabled = false;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().AddForce(Vector3.zero);
    }
}
