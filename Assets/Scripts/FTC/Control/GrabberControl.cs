using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Control;
using Photon.Pun;

public class GrabberControl : MonoBehaviourPun
{
    public CommandProcessor Commands = new CommandProcessor();

    private bool grabbing = false;
    public int pointsPerGoal = 0;
    private string tagOfGameObject1 = "RedWobble";
    private string tagOfGameObject2 = "BlueWobble";

    private GameObject wobble = null;
    private GameObject field;
    public Transform robot;
    public BoxCollider wobblePlaceholder;
    public Vector3 wobblePosition = Vector3.zero;

    void OnTriggerEnter(Collider collision)
    {
        if ((collision.tag == tagOfGameObject1 || collision.tag == tagOfGameObject2) && wobble == null)
        {
            wobble = collision.gameObject;
        }
    }

    public void startGrab()
    {
        if (wobble != null && !grabbing)
        {
            grabbing = true;
            

            if (PhotonNetwork.IsConnected)
            {
                wobble.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
                PhotonNetwork.SendAllOutgoingCommands();

                

                photonView.RPC("NetworkGrab", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void NetworkGrab(PhotonMessageInfo info)
    {
        wobble.transform.SetParent(info.photonView.gameObject.transform);        
        wobble.transform.localPosition = new Vector3(0f, -0.39f, 0.3f);

        wobble.GetComponent<Rigidbody>().isKinematic = true;        
        wobble.GetComponent<PhotonRigidbodyView>().enabled = false;
        wobble.GetComponent<PhotonTransformView>().enabled = false;
    }

    public void lift()
    {
        if (wobble != null && grabbing)
        {
            wobble.transform.localPosition = new Vector3(0f, -0.39f, 0.3f);

            if (wobble.GetComponent<RedWobble>())
            {
                wobble.GetComponent<RedWobble>().UnscoreWobble();
            }
            else if (wobble.GetComponent<BlueWobble>())
            {
                wobble.GetComponent<BlueWobble>().UnscoreWobble();
            }
        }
    }

    public void stopGrab()
    {
        if (wobble != null && grabbing)
        {
            grabbing = false;            

            if (PhotonNetwork.IsConnected)
            {
                wobble.transform.SetParent(null);
                photonView.RPC("NetworkStopGrab", RpcTarget.All);
            }         
        }
        wobble = null;
    }

    [PunRPC]
    public void NetworkStopGrab(PhotonMessageInfo info)
    {
        wobble.transform.SetParent(null);
        wobble.GetComponent<PhotonRigidbodyView>().enabled = true;
        wobble.GetComponent<PhotonTransformView>().enabled = false;
        wobble.GetComponent<Rigidbody>().isKinematic = false;
    }
}
