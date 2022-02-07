using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WhiteLineGoal : MonoBehaviour
{
    private ScoreKeeper scoreKeeper;
    public int pointsPerGoal = 0;
    public string tagOfGameObject1 = "BlueRobot";
    public string tagOfGameObject2 = "RedRobot";

    private bool inZone = false;

    private GameTimer gameTimer;

    void Awake()
    {
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        gameTimer = GameObject.Find("ScoreKeeper").GetComponent<GameTimer>();
    }

    void OnTriggerEnter(Collider collision)
    {
        PhotonView colView = collision.GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && !colView.IsMine)
            return;
        if ((collision.tag == tagOfGameObject1 || collision.tag == tagOfGameObject2) && inZone == false && gameTimer.getGameType() == "auto")
        {
            pointsPerGoal = 5;

            inZone = true;
            if (!PhotonNetwork.IsConnected)
            {
                if (collision.tag == tagOfGameObject1)
                    scoreKeeper.addScoreBlue(pointsPerGoal);
                else
                    scoreKeeper.addScoreRed(pointsPerGoal);
            }
            else
            {
                if (collision.tag == tagOfGameObject1)
                    colView.RPC("addScoreBlue", RpcTarget.AllBuffered, pointsPerGoal);
                else
                    colView.RPC("addScoreRed", RpcTarget.AllBuffered, pointsPerGoal);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        PhotonView colView = collision.GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && !colView.IsMine)
            return;
        if ((collision.tag == tagOfGameObject1 || collision.tag == tagOfGameObject2) && inZone == false && gameTimer.getGameType() == "auto")
        {
            pointsPerGoal = 5;

            inZone = false;
            if (!PhotonNetwork.IsConnected)
            {
                if (collision.tag == tagOfGameObject1)
                    scoreKeeper.addScoreBlue(-pointsPerGoal);
                else
                    scoreKeeper.addScoreRed(-pointsPerGoal);
            }
            else
            {
                if (collision.tag == tagOfGameObject1)
                    colView.RPC("addScoreBlue", RpcTarget.AllBuffered, -pointsPerGoal);
                else
                    colView.RPC("addScoreRed", RpcTarget.AllBuffered, -pointsPerGoal);
            }
        }
    }
}
