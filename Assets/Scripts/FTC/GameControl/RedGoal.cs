using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGoal : MonoBehaviour
{
    private ScoreKeeper scoreKeeper;
    public int pointsPerGoal = 0;
    public string tagOfGameObject = "Ring";

    public enum goal { low, mid, high, power }
    public goal goalType;

    private GameTimer gameTimer;
    private AudioManager audioManager;

    private GameObject particle;
    private ParticleSystem partSystem;

    void Awake()
    {
        particle = GameObject.Find("ScoreFlash-Yellow");
        partSystem = particle.GetComponent<ParticleSystem>();
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        gameTimer = GameObject.Find("ScoreKeeper").GetComponent<GameTimer>();
        audioManager = GameObject.Find("ScoreKeeper").GetComponent<AudioManager>();
    }

    void OnTriggerEnter(Collider collision)
    {
        PhotonView colView = collision.GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && !colView.IsMine)
            return;
        if (collision.tag == tagOfGameObject)
        {
            if (goalType == goal.low)
            {
                Destroy(collision.gameObject.transform.parent.gameObject);
                audioManager.playRingBounce();
                pointsPerGoal = 2;
                if (gameTimer.getGameType() == "auto")
                    pointsPerGoal = 3;
            }
            if (goalType == goal.mid)
            {
                Destroy(collision.gameObject.transform.parent.gameObject);
                audioManager.playRingBounce();
                pointsPerGoal = 4;
                if (gameTimer.getGameType() == "auto")
                    pointsPerGoal = 6;
            }
            if (goalType == goal.high)
            {
                Destroy(collision.gameObject.transform.parent.gameObject);
                audioManager.playRingBounce();
                pointsPerGoal = 6;
                if (gameTimer.getGameType() == "auto")
                    pointsPerGoal = 12;
            }
            if (goalType == goal.power)
            {
                pointsPerGoal = 0;
                if (gameTimer.getGameType() == "auto" || gameTimer.getGameType() == "end" || gameTimer.getGameType() == "freeplay")
                {
                    Destroy(collision.gameObject.transform.parent.gameObject);
                    audioManager.playRingBounce();
                    pointsPerGoal = 15;
                }
                    
            }

            if (!PhotonNetwork.IsConnected)
            {
                scoreKeeper.addScoreRed(pointsPerGoal);
            }
            else
            {
                colView.RPC("addScoreRed", RpcTarget.AllBuffered, pointsPerGoal);
            }

            particle.transform.position = transform.position;
            partSystem.Play();
        }
    }
}
