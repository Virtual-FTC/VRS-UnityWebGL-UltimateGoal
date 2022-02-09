using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RedWobbleGoal : MonoBehaviour
{
    public int pointsPerGoal = 0;
    public string tagOfGameObject = "RedWobble";

    public string goalType = "A";

    private GameTimer gameTimer;

    void Start()
    {
        gameTimer = ScoreKeeper._Instance.GetComponent<GameTimer>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;
        if (collision.tag == tagOfGameObject)
        {
            if (gameTimer.getGameSetup() == goalType && gameTimer.getGameType() == "auto")
                pointsPerGoal = 15;
            else if (goalType == "line" && gameTimer.getGameType() == "end")
                pointsPerGoal = 5;
            else if (goalType == "drop" && gameTimer.getGameType() == "end")
                pointsPerGoal = 20;
            else
                pointsPerGoal = 0;

            ScoreKeeper._Instance.addScoreRed(pointsPerGoal);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;
        if (collision.tag == tagOfGameObject)
        {
            if (gameTimer.getGameSetup() == goalType && gameTimer.getGameType() == "auto")
                pointsPerGoal = 15;
            else if (goalType == "line" && gameTimer.getGameType() == "end")
                pointsPerGoal = 5;
            else if (goalType == "drop" && gameTimer.getGameType() == "end")
                pointsPerGoal = 20;
            else
                pointsPerGoal = 0;

            ScoreKeeper._Instance.addScoreRed(-pointsPerGoal);
        }
    }
}
