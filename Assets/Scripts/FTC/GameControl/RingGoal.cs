using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingGoal : MonoBehaviour
{
    public string tagOfGameObject = "Ring";

    public enum goal { low, mid, high, power }
    public goal goalType;
    public enum goalColor { red, blue }
    public goalColor goalCol;
    [Tooltip("reference to the power goal mesh, leave empty for other goal types")]
    public Transform powerGoal;

    private int pointsPerGoal = 0;
    private bool powerGoalUsable;
    private GameTimer gameTimer;
    private AudioManager audioManager;

    private GameObject particle;
    private ParticleSystem partSystem;

    void Start()
    {
        particle = GameObject.Find("ScoreFlash-Yellow");
        partSystem = particle.GetComponent<ParticleSystem>();
        gameTimer = ScoreKeeper._Instance.GetComponent<GameTimer>();
        audioManager = GameObject.Find("ScoreKeeper").GetComponent<AudioManager>();
        powerGoalUsable = true;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (gameTimer.getTimer() <= 0)
            return;
        PhotonView colView = collision.GetComponentInParent<PhotonView>();
        if (PhotonNetwork.IsConnected && !colView.IsMine)
            return;
        if (collision.tag == tagOfGameObject)
        {
            if (goalType == goal.low)
            {
                scoreRing(collision.gameObject.transform.parent.gameObject, ScoreKeeper._Instance.FreeplayRingLow, ScoreKeeper._Instance.AutoRingLow);
            }
            if (goalType == goal.mid)
            {
                scoreRing(collision.gameObject.transform.parent.gameObject, ScoreKeeper._Instance.FreeplayRingMid, ScoreKeeper._Instance.AutoRingMid);
            }
            if (goalType == goal.high)
            {
                scoreRing(collision.gameObject.transform.parent.gameObject, ScoreKeeper._Instance.FreeplayRingHigh, ScoreKeeper._Instance.AutoRingHigh);
            }
            if (goalType == goal.power && gameTimer.getTimer() < 30f)
            {
                pointsPerGoal = 0;
                if (powerGoalUsable)
                {
                    if (gameTimer.getGameType() == "auto" || gameTimer.getGameType() == "end" || gameTimer.getGameType() == "freeplay")
                    {
                        pointsPerGoal = ScoreKeeper._Instance.PowerGoal;
                        collision.gameObject.transform.parent.gameObject.GetComponent<PhotonView>().RPC("DestroyRing", RpcTarget.MasterClient);
                        audioManager.playRingBounce();
                        powerGoal.RotateAround(powerGoal.position, Vector3.left, ScoreKeeper._Instance.powerGoalKnockback);
                        powerGoalUsable = false;
                    }
                }
            }
            if(goalCol == goalColor.red)
                ScoreKeeper._Instance.addScoreRed(pointsPerGoal);
            else
                ScoreKeeper._Instance.addScoreBlue(pointsPerGoal);

            particle.transform.position = transform.position;
            partSystem.Play();
        }
    }
    void scoreRing(GameObject ring, int pointA, int pointB)
    {
        if(PhotonNetwork.IsConnected)
            ring.GetComponent<PhotonView>().RPC("DestroyRing", RpcTarget.AllBuffered);

        audioManager.playRingBounce();
        pointsPerGoal = pointA;
        if (gameTimer.getGameType() == "auto")
            pointsPerGoal = pointB;
    }
}
