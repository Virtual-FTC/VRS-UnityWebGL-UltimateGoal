﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlueGoal : MonoBehaviour
{
    private ScoreKeeper scoreKeeper;
    public int pointsPerGoal = 0;
    public string tagOfGameObject = "Ring";

    public string goalType = "";
    public enum goalType2 { low, mid, high, power }
    public goalType2 dropDownList;

    private GameTimer gameTimer;
    private AudioManager audioManager;

    private GameObject particle;
    private ParticleSystem partSystem;

    //public Material mat;

    void Awake()
    {
        particle = GameObject.Find("ScoreFlash-Blue");
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
                PhotonNetwork.Destroy(collision.gameObject.transform.parent.gameObject);
                audioManager.playRingBounce();
                pointsPerGoal = 2;
                if (gameTimer.getGameType() == "auto")
                    pointsPerGoal = 3;
            }
            if (goalType == "mid")
            {
                PhotonNetwork.Destroy(collision.gameObject.transform.parent.gameObject);
                audioManager.playRingBounce();
                pointsPerGoal = 4;
                if (gameTimer.getGameType() == "auto")
                    pointsPerGoal = 6;
            }
            if (goalType == "high")
            {
                PhotonNetwork.Destroy(collision.gameObject.transform.parent.gameObject);
                audioManager.playRingBounce();
                pointsPerGoal = 6;
                if (gameTimer.getGameType() == "auto")
                    pointsPerGoal = 12;
            }
            if (goalType == "power")
            {
                pointsPerGoal = 0;
                if (gameTimer.getGameType() == "auto" || gameTimer.getGameType() == "end" || gameTimer.getGameType() == "freeplay")
                {
                    pointsPerGoal = 15;
                    PhotonNetwork.Destroy(collision.gameObject.transform.parent.gameObject);
                    audioManager.playRingBounce();
                }
            }

            scoreKeeper.addScoreBlue(pointsPerGoal);

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
}
