using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RingController : MonoBehaviour
{
    public PhotonView thisView;
    public Rigidbody rb;

    public float minVelocityForMovement;

    private PhotonTransformView transView;

    private void Start()
    {
        transView = GetComponent<PhotonTransformView>();
        transView.enabled = false;
    }

    [PunRPC]
    public void DestroyRing()
    {
        if (thisView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        //print(rb.velocity + " : "  + Vector3.Distance(rb.velocity, Vector3.zero));
        if (IsBetween(rb.velocity.x, -minVelocityForMovement, minVelocityForMovement))
            return;
        transView.enabled = true;
        //print("RING IS MOVING");
        //print(rb.velocity);
    }
    public bool IsBetween(double testValue, double bound1, double bound2)
    {
        if (bound1 > bound2)
            return testValue >= bound2 && testValue <= bound1;
        return testValue >= bound1 && testValue <= bound2;
    }
}
