using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WobbleGoal : MonoBehaviour
{
    public int pointsPerGoal = 0;
    private string tagOfGameObject;

    public enum goalColor { red, blue }
    public goalColor goalCol;
    public string goalType = "A";

    private GameTimer gameTimer;

    void Start()
    {
        gameTimer = ScoreKeeper._Instance.GetComponent<GameTimer>();
        if (goalCol == goalColor.red)
            tagOfGameObject = "RedWobble";
        else
            tagOfGameObject = "BlueWobble";
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

            if (goalCol == goalColor.red)
                ScoreKeeper._Instance.addScoreRed(pointsPerGoal);
            else
                ScoreKeeper._Instance.addScoreBlue(pointsPerGoal);
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

            if (goalCol == goalColor.red)
                ScoreKeeper._Instance.addScoreRed(-pointsPerGoal);
            else
                ScoreKeeper._Instance.addScoreBlue(-pointsPerGoal);
        }
    }
}
