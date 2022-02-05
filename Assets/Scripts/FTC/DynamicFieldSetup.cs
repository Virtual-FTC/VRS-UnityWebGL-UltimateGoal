using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DynamicFieldSetup : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("DYNAMIC FIELD SETUP");

        foreach (PhotonView pv in GetComponentsInChildren<PhotonView>())
        {
            pv.gameObject.transform.parent = null;
        }
    }
}
