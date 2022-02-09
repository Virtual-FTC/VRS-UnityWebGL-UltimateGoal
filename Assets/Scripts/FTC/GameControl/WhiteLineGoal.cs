using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WhiteLineGoal : MonoBehaviour
{
    public int pointsPerGoal = 0;
    public string tagOfGameObject1 = "BlueRobot";
    public string tagOfGameObject2 = "RedRobot";

    private bool inZone = false;

    private GameTimer gameTimer;

    void Start()
    {
        gameTimer = ScoreKeeper._Instance.GetComponent<GameTimer>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;
        if ((collision.tag == tagOfGameObject1 || collision.tag == tagOfGameObject2) && inZone == false && gameTimer.getGameType() == "auto")
        {
            pointsPerGoal = 5;

            inZone = true;

            if (collision.tag == tagOfGameObject1)
                ScoreKeeper._Instance.addScoreBlue(pointsPerGoal);
            else
                ScoreKeeper._Instance.addScoreRed(pointsPerGoal);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;
        if ((collision.tag == tagOfGameObject1 || collision.tag == tagOfGameObject2) && inZone == false && gameTimer.getGameType() == "auto")
        {
            pointsPerGoal = 5;

            inZone = false;

            if (collision.tag == tagOfGameObject1)
                ScoreKeeper._Instance.addScoreBlue(-pointsPerGoal);
            else
                ScoreKeeper._Instance.addScoreRed(-pointsPerGoal);
        }
    }
}
