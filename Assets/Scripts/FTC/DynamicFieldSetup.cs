using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DynamicFieldSetup : MonoBehaviour
{
    private void Start()
    {   
        foreach (RingController ring in GetComponentsInChildren<RingController>())
        {
            Transform ringTrans = ring.gameObject.transform;            
            Destroy(ring.gameObject);

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Ring"), ringTrans.position, Quaternion.identity);
        }
        
        foreach (BlueWobble wobble in GetComponentsInChildren<BlueWobble>())
        {
            Transform wobTrans = wobble.gameObject.transform;            
            Destroy(wobble.gameObject);

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BlueWobble"), wobTrans.position, Quaternion.identity);
        }
        
        foreach (RedWobble wobble in GetComponentsInChildren<RedWobble>())
        {
            Transform wobTrans = wobble.gameObject.transform;
            Destroy(wobble.gameObject);

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RedWobble"), wobTrans.position, Quaternion.identity);
        }
    }
}
