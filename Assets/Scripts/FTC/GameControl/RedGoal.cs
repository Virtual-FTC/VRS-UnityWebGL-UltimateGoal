using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RedGoal : MonoBehaviour
{
    private ScoreKeeper scoreKeeper;
    public int pointsPerGoal = 0;
    public string tagOfGameObject = "Ring";

    public string goalType = "";

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
        if (collision.tag == tagOfGameObject)
        {
            if (goalType == "low")
            {
                destroyRing(collision.gameObject.transform.parent.gameObject, 2, 3);
            }
            if (goalType == "mid")
            {
                destroyRing(collision.gameObject.transform.parent.gameObject, 4, 6);
            }
            if (goalType == "high")
            {
                destroyRing(collision.gameObject.transform.parent.gameObject, 6, 12);
            }
            if (goalType == "power")
            {
                pointsPerGoal = 0;
                if (gameTimer.getGameType() == "auto" || gameTimer.getGameType() == "end" || gameTimer.getGameType() == "freeplay")
                {
                    pointsPerGoal = 15;
                    collision.gameObject.transform.parent.gameObject.GetComponent<PhotonView>().RPC("DestroyRing", RpcTarget.MasterClient);
                    audioManager.playRingBounce();
                }
            }
            scoreKeeper.addScoreRed(pointsPerGoal);

            particle.transform.position = transform.position;
            partSystem.Play();
        }
    }
    void destroyRing(GameObject ring, int pointA, int pointB)
    {
        ring.GetComponent<PhotonView>().RPC("DestroyRing", RpcTarget.AllBuffered);
        audioManager.playRingBounce();
        pointsPerGoal = pointA;
        if (gameTimer.getGameType() == "auto")
            pointsPerGoal = pointB;
    }
}
