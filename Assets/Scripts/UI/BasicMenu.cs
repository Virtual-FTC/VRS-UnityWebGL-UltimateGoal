using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicMenu : MonoBehaviour
{
    public vrs_messenger messenger;
    public void ChangeToScene(int sceneIndex)
    {
        if (sceneIndex == 0 && Photon.Pun.PhotonNetwork.IsConnected)
        {
            Photon.Pun.PhotonNetwork.Disconnect();
        }
        SceneManager.LoadScene(sceneIndex);
    }

    public void changeSinglePlayer()
    {
        SceneManager.LoadScene(3);
    }
}
