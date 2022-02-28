using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WobbleGoal : MonoBehaviour
{
    private int pointsPerGoal = 0;
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
                pointsPerGoal = ScoreKeeper._Instance.WobbleAuto;
            else if (goalType == "line" && gameTimer.getGameType() == "end")
                pointsPerGoal = ScoreKeeper._Instance.WobbleLine;
            else if (goalType == "drop" && gameTimer.getGameType() == "end")
                pointsPerGoal = ScoreKeeper._Instance.WobbleDrop;
            else
            {
                pointsPerGoal = 0;
            }
            print("scoring points: " + pointsPerGoal);
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
                pointsPerGoal = ScoreKeeper._Instance.WobbleAuto;
            else if (goalType == "line" && gameTimer.getGameType() == "end")
                pointsPerGoal = ScoreKeeper._Instance.WobbleLine;
            else if (goalType == "drop" && gameTimer.getGameType() == "end")
                pointsPerGoal = ScoreKeeper._Instance.WobbleDrop;
            else
                pointsPerGoal = 0;

            print("REMOVING points: " + pointsPerGoal);
            if (goalCol == goalColor.red)
                ScoreKeeper._Instance.addScoreRed(-pointsPerGoal);
            else
                ScoreKeeper._Instance.addScoreBlue(-pointsPerGoal);
        }
    }
}
