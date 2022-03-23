﻿using System.Collections;
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
        //if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
          //  return;
        if (collision.tag == tagOfGameObject)
        {
            if (gameTimer.getGameSetup() == goalType && gameTimer.getGameType() == "auto")
                Score(collision, ScoreKeeper._Instance.WobbleAuto);
            else if (goalType == "line" && gameTimer.getGameType() == "end")
                Score(collision, ScoreKeeper._Instance.WobbleLine);
            else if (goalType == "drop" && gameTimer.getGameType() == "end")
                Score(collision, ScoreKeeper._Instance.WobbleDrop);
            else
                pointsPerGoal = 0;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        //if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
          //  return;
        if (collision.tag == tagOfGameObject)
        {
            if (gameTimer.getGameSetup() == goalType && gameTimer.getGameType() == "auto")
                Unscore(collision, ScoreKeeper._Instance.WobbleAuto);
            else if (goalType == "line" && gameTimer.getGameType() == "end")
                Unscore(collision, ScoreKeeper._Instance.WobbleLine);
            else if (goalType == "drop" && gameTimer.getGameType() == "end")
                Unscore(collision, ScoreKeeper._Instance.WobbleDrop);
            else
                pointsPerGoal = 0;

        }
    }

    private void Score(Collider collision, int points)
    {
        if (!(goalType == "line" || goalType == "drop"))
        {
            if (goalCol == goalColor.red)
                collision.GetComponent<RedWobble>().ScoreWobble(points);
            else
                collision.GetComponent<BlueWobble>().ScoreWobble(points);
        }
        else
        {
            if (goalCol == goalColor.red)
                collision.GetComponent<RedWobble>().ScoreWobbleTeleop(goalType, points);
            else
                collision.GetComponent<BlueWobble>().ScoreWobbleTeleop(goalType, points);
        }
    }

    private void Unscore(Collider collision, int points)
    {
        if (!(goalType == "line" || goalType == "drop"))
        {
            if (goalCol == goalColor.red)
                collision.GetComponent<RedWobble>().UnscoreWobble(-points);
            else
                collision.GetComponent<BlueWobble>().UnscoreWobble(-points);
    }
        else
        {
            if (goalCol == goalColor.red)
                collision.GetComponent<RedWobble>().UnscoreWobbleTeleop(goalType, -points);
            else
                collision.GetComponent<BlueWobble>().UnscoreWobbleTeleop(goalType, -points);
}
    }
}
