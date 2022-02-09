using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueGoal : MonoBehaviour
{
    public int pointsPerGoal = 0;
    public string tagOfGameObject = "Ring";

    public enum goal { low, mid, high, power }
    public goal goalType;

    private GameTimer gameTimer;
    private AudioManager audioManager;

    private GameObject particle;
    private ParticleSystem partSystem;

    //public Material mat;

    void Start()
    {
        particle = GameObject.Find("ScoreFlash-Blue");
        partSystem = particle.GetComponent<ParticleSystem>();
        gameTimer = ScoreKeeper._Instance.GetComponent<GameTimer>();
        audioManager = GameObject.Find("ScoreKeeper").GetComponent<AudioManager>();
    }


    void OnTriggerEnter(Collider collision)
    {
        PhotonView colView = collision.GetComponentInParent<PhotonView>();
        if (PhotonNetwork.IsConnected && !colView.IsMine)
            return;
        if (collision.tag == tagOfGameObject)
        {
            if (goalType == goal.low)
            {
                destroyRing(collision.gameObject.transform.parent.gameObject, 2, 3);
            }
            if (goalType == goal.mid)
            {
                destroyRing(collision.gameObject.transform.parent.gameObject, 4, 6);
            }
            if (goalType == goal.high)
            {
                destroyRing(collision.gameObject.transform.parent.gameObject, 6, 12);
            }
            if (goalType == goal.power)
            {
                pointsPerGoal = 0;
                if (gameTimer.getGameType() == "auto" || gameTimer.getGameType() == "end" || gameTimer.getGameType() == "freeplay")
                {
                    pointsPerGoal = 15;
                    collision.gameObject.transform.parent.gameObject.GetComponent<PhotonView>().RPC("DestroyRing", RpcTarget.MasterClient);
                    audioManager.playRingBounce();
                }
            }

            if (!PhotonNetwork.IsConnected)
            {
                ScoreKeeper._Instance.addScoreBlue(pointsPerGoal);
            }
            else
            {
                ScoreKeeper._Instance.thisView.RPC("addScoreBlue", RpcTarget.AllBuffered, pointsPerGoal);
            }

            //mat.EnableKeyword("_EMISSION");
            //Color myColor = new Color();
            //var color = "#3A2CDC";
            //ColorUtility.TryParseHtmlString(color, out myColor);
            //mat.color = myColor;
            //mat.SetColor("_EmissionColor", myColor);

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
