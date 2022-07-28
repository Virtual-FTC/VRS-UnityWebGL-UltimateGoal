using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WhiteLineGoal : MonoBehaviour
{
    public string tagOfGameObject1 = "BlueRobot";
    public string tagOfGameObject2 = "RedRobot";

    private bool inZone = false;
    private bool pointsAdded = false;
    private float timeInZone = 0.0f;

    private GameTimer gameTimer;

    void Start()
    {
        gameTimer = ScoreKeeper._Instance.GetComponent<GameTimer>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if(PhotonNetwork.IsConnected)
        {
            if(collision.tag == "Player")
            {
                inZone = true;
            }
        }
        else
        {
            if(collision.tag == "Player")
            {
                inZone = true;
            }
        }
    }
    void OnTriggerStay(Collider collision)
    {
        if(PhotonNetwork.IsConnected)
        {
            if(inZone)
            {
                if(!pointsAdded)
                {
                    timeInZone += Time.fixedDeltaTime;
                    if(timeInZone > 5.0f && !pointsAdded)
                    {
                        pointsAdded = true;
                        object team;
                        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PLAYERPROPS.PLAYER_TEAM,out team);
                        if((int)team == 0)
                            ScoreKeeper._Instance.addScoreBlue(ScoreKeeper._Instance.WhiteLineGoal);
                        else
                            ScoreKeeper._Instance.addScoreRed(ScoreKeeper._Instance.WhiteLineGoal);
                    }
                }
            }
        }
        else
        {
            if(inZone)
            {
                if(!pointsAdded)
                {
                    timeInZone += Time.fixedDeltaTime;
                    if(timeInZone > 5.0f && !pointsAdded)
                    {
                        pointsAdded = true;
                        ScoreKeeper._Instance.addScoreBlue(ScoreKeeper._Instance.WhiteLineGoal);
                    }
                }
            }
        }

    }

    private void OnTriggerExit(Collider collision)
    {
        if(PhotonNetwork.IsConnected)
        {
            if(collision.tag == "Player")
            {
                inZone = false;
                timeInZone = 0.0f;
                if(pointsAdded)
                {
                    pointsAdded = false;
                    object team;
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PLAYERPROPS.PLAYER_TEAM,out team);
                    if((int)team == 0)
                        ScoreKeeper._Instance.addScoreBlue(-ScoreKeeper._Instance.WhiteLineGoal);
                    else
                        ScoreKeeper._Instance.addScoreRed(-ScoreKeeper._Instance.WhiteLineGoal);

                }
            }
        }
        else
        {
            if(collision.tag == "Player")
            {
                inZone = false;
                timeInZone = 0.0f;
                if(pointsAdded)
                {
                    pointsAdded = false;
                    ScoreKeeper._Instance.addScoreBlue(-ScoreKeeper._Instance.WhiteLineGoal);
                }
            }
        }

    }
}
