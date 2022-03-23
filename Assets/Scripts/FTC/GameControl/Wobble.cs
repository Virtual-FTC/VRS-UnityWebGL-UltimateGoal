using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Wobble : MonoBehaviourPun
{
    [PunRPC]
    public void NetworkGrab(Player p, PhotonMessageInfo info)
    {
        var robot = UnityEngine.Object.FindObjectsOfType<RobotController>();
        foreach (var RobotController in robot)
        {
            //Objects in the scene don't have an owner, its means view.owner will be null
            if (RobotController.gameObject.GetPhotonView().Owner == p)
            {
                transform.SetParent(RobotController.gameObject.transform);
            }
        }

        this.transform.localPosition = new Vector3(0f, -0.39f, 0.05f);
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
