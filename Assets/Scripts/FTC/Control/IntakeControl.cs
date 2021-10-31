using Assets.Scripts.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntakeControl : MonoBehaviour
{
    public CommandProcessor Commands = new CommandProcessor();

    [Header("Ball Pickup")]
    public int maxNumberBalls = 5;
    public int numBalls = 3;
    public float timeOfBallContact = 1.0f;
    public string[] coliderTags = {"PowerCell" };
    private int colliderTagIndex;

    [Header("Intake Motor")]
    public float desiredVelocity = 0f;

    private float timer = 0.0f;

    private int resetNum = 3;

    public GameObject[] rings;

    private GameObject lastRing;

    // Intake Motor Control
    void Start()
    {
        retractIntake();
    }


    public void deployIntake()
    {
        //var hinge = GetComponent<HingeJoint>();
        //var motor = hinge.motor;
        //motor.targetVelocity = wantedVelocity;

        //hinge.motor = motor;
    }

    public void retractIntake()
    {
        //var hinge = GetComponent<HingeJoint>();
        //var motor = hinge.motor;
        //motor.targetVelocity = -wantedVelocity;

        //hinge.motor = motor;
        desiredVelocity = 0f;
    }

    // Ball Pickup
    void OnTriggerEnter(Collider collision)
    {
        if (desiredVelocity != 0)
        {
            print("Test");
            colliderTagIndex = -1;
            foreach (string coliderTag in coliderTags)
            {
                colliderTagIndex++;
                if (collision.tag == coliderTag && numBalls < maxNumberBalls)
                {
                    timer = Time.time;
                    break;
                }
            }
        }
        
    }

    void OnTriggerStay(Collider collision)
    {
        if (desiredVelocity != 0 && collision.gameObject != lastRing)
        {
            foreach (string coliderTag in coliderTags)
            {
                if (collision.tag == coliderTag && numBalls < maxNumberBalls && Time.time - timer >= timeOfBallContact)
                {
                    numBalls++;
                    //resetBalls();
                    //rings[colliderTagIndex].SetActive(true);
                    lastRing = collision.gameObject;
                    if (Photon.Pun.PhotonNetwork.IsConnected)
                    {
                        collision.gameObject.GetComponent<Photon.Pun.PhotonView>().RPC("DestroyRing", Photon.Pun.RpcTarget.All);
                        Photon.Pun.PhotonNetwork.Destroy(collision.gameObject);
                    }
                    else
                    {
                        Destroy(collision.gameObject);
                    }
                }
            }
        }
        
    }

    public void subtractBall()
    {
        numBalls--;
        //rings[numBalls].SetActive(false);
    }

    public void resetBalls()
    {
        for (int x = 0; x < 3; x++)
        {
            rings[x].SetActive(false);
        }
    }

    public int getNumberBalls()
    {
        return numBalls;
    }

    public void setResetNum(int num)
    {
        resetNum = num;
    }

    public void setVelocity(float x)
    {
        desiredVelocity = x;
    }
}
