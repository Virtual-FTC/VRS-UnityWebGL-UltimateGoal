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
        if ((collision.tag == tagOfGameObject1 || collision.tag == tagOfGameObject2) && !grabbing)
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
                wobble.GetPhotonView().RPC("NetworkGrab", RpcTarget.All, PhotonNetwork.LocalPlayer);
                PhotonNetwork.SendAllOutgoingCommands();
            }
            else
            {
                wobble.transform.SetParent(robot);
                wobble.transform.localPosition = new Vector3(0f, -0.39f, 0.05f);
                wobble.GetComponent<Rigidbody>().isKinematic = true;
                wobble.GetComponent<PhotonRigidbodyView>().enabled = false;
                wobble.GetComponent<PhotonTransformView>().enabled = true;
            }
        }

        if (wobble != null && grabbing)
        {
            wobble.transform.localPosition = new Vector3(0f, -0.39f, 0.05f);
        }
    }

    public void lift()
    {
        if (wobble != null && grabbing)
        {
            wobble.transform.localPosition = new Vector3(0f, -0.39f, 0.3f);

            if (PhotonNetwork.IsConnected)
            {
                wobble.GetComponent<PhotonView>().RPC("UnscoreWobble", RpcTarget.AllBuffered);
                wobble.GetComponent<PhotonView>().RPC("UnscoreWobbleTeleop", RpcTarget.AllBuffered);
            }
            else if (wobble.GetComponent<RedWobble>())
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

            if (PhotonNetwork.IsConnected)
            {
                wobble.GetPhotonView().RPC("NetworkStopGrab", RpcTarget.All);
                photonView.RPC("RPC_StopGrab", RpcTarget.All);
                PhotonNetwork.SendAllOutgoingCommands();
            }
            else
            {
                wobble.transform.SetParent(null);
                wobble.GetComponent<PhotonRigidbodyView>().enabled = true;
                wobble.GetComponent<PhotonTransformView>().enabled = false;
                wobble.GetComponent<Rigidbody>().isKinematic = false;
                wobble.GetComponent<Rigidbody>().AddForce(Vector3.zero);
            }
        }
        grabbing = false;
        wobble = null;
    }
    
    [PunRPC]
    public void RPC_StopGrab()
    {
        wobble = null;
    }
}
