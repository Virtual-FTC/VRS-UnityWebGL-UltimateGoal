using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Control;
using Photon.Pun;

public class GrabberControl : MonoBehaviour
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
            wobble.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonNetwork.SendAllOutgoingCommands();
            wobble.transform.SetParent(robot);
            wobble.GetComponent<Rigidbody>().isKinematic = true;
            wobble.GetComponent<PhotonRigidbodyView>().enabled = false;
            wobble.GetComponent<PhotonTransformView>().enabled = true;
            //wobble.GetComponent<PhotonView>().RPC("swapPhotonViews", RpcTarget.AllBuffered, false);
            wobble.transform.localPosition = new Vector3(0f,-0.39f, 0.05f);
            //wobblePlaceholder.enabled = true;
        }
    }

    public void lift()
    {
        if (wobble != null && grabbing)
        {
            wobble.transform.localPosition = new Vector3(0f, -0.39f, 0.3f);
            //wobblePlaceholder.enabled = false;
        }
    }

    public void stopGrab()
    {
        if (wobble != null && grabbing)
        {
            grabbing = false;
            wobble.transform.SetParent(null);
            wobble.GetComponent<Rigidbody>().isKinematic = false;
            wobble.GetComponent<PhotonRigidbodyView>().enabled = true;
            wobble.GetComponent<PhotonTransformView>().enabled = false;
            //wobble.GetComponent<PhotonView>().RPC("swapPhotonViews", RpcTarget.AllBuffered, true);
            //wobblePlaceholder.enabled = false;
        }
        wobble = null;
    }
}
