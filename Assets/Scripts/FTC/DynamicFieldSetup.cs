using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DynamicFieldSetup : MonoBehaviour
{
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        foreach (RingController ring in GetComponentsInChildren<RingController>())
        {
            Transform ringTrans = ring.gameObject.transform;
            PhotonNetwork.Destroy(ring.GetComponent<PhotonView>());
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Ring"), ringTrans.position, Quaternion.identity);
        }
        foreach (BlueWobble wobble in GetComponentsInChildren<BlueWobble>())
        {
            Transform wobTrans = wobble.gameObject.transform;
            PhotonNetwork.Destroy(wobble.GetComponent<PhotonView>());
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BlueWobble"), wobTrans.position, Quaternion.identity);
        }
        foreach (RedWobble wobble in GetComponentsInChildren<RedWobble>())
        {
            Transform wobTrans = wobble.gameObject.transform;
            PhotonNetwork.Destroy(wobble.GetComponent<PhotonView>());
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RedWobble"), wobTrans.position, Quaternion.identity);
        }
    }
}
