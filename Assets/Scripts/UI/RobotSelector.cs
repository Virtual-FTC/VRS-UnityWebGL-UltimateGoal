using System.Collections;
using System.Collections.Generic;
using  System.IO;
using UnityEngine;
using Photon.Pun;

public class RobotSelector : MonoBehaviour
{
    public delegate void RobotChangedEvent(GameObject newRobot);
    public RobotChangedEvent OnRobotChanged;

    public string[] prefabNames;
    public GameObject[] spawnPositions;

    private bool isDisplaying = false;
    public GameObject displayButton;
    public GameObject buttonGroup;

    public GameObject[] robotPrefabs;
    void Start()
    {
        buttonGroup.SetActive(false);
    }
    public void toggleRobotOptions()
    {
        isDisplaying = !isDisplaying;
        displayButton.SetActive(!isDisplaying);
        buttonGroup.SetActive(isDisplaying);
    }
    public void changeRobot(int robotType)
    {
        if(PhotonNetwork.IsConnected)
        {
            foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if(player.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                {
                    PhotonNetwork.Destroy(player);
                }
            }
            int spawnPos = (int)PhotonNetwork.LocalPlayer.CustomProperties[PLAYERPROPS.PLAYER_POS];


            var robot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabNames[robotType]), spawnPositions[spawnPos].transform.position, spawnPositions[spawnPos].transform.rotation, 0);
            robot.GetComponent<RobotController>().setStartPosition(spawnPositions[spawnPos].transform);

            toggleRobotOptions();
            OnRobotChanged?.Invoke(robot);
        }
        else
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));

            toggleRobotOptions();
            OnRobotChanged?.Invoke(robotPrefabs[robotType]);
        }
        
    }
}
