using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Random = System.Random;
using UnityEngine.SceneManagement;
using System.Collections;

public class FieldManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private IntakeControl intake;
    private CameraPosition camera;

    private GameTimer gameTimer;

    private bool currentGameStart = false;
    private string currentGameSetup = "A";
    private string currentGameType = "";
    private int currentCam = 0;

    public GameObject[] setupPrefab;
    private GameObject setup;

    public Transform[] spawnPositions;

    private GameObject robot;
    public GameObject robotPrefab;

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Using Photon");
        }
        else
        {
            robot = (GameObject)Instantiate(robotPrefab, spawnPositions[0].transform.position, spawnPositions[0].transform.rotation);
            robot.GetComponent<RobotController>().setStartPosition(spawnPositions[0].transform);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            var list = GameObject.FindGameObjectsWithTag("Player");
            for (int x = 0; x < list.Length; x++)
            {
                if (list[x].GetComponent<PhotonView>().IsMine)
                {
                    intake = list[x].GetComponentInChildren<IntakeControl>();
                }
            }
        }
        else
        {
            intake = GameObject.Find("Intake").GetComponent<IntakeControl>();
        }
        //camera = GameObject.Find("Camera").GetComponent<CameraPosition>();
        gameTimer = GameObject.Find("ScoreKeeper").GetComponent<GameTimer>();
        //camera.switchCamera(MultiplayerSetting.multiplayerSetting.getCamSetup());

        resetField();
        if (MultiplayerSetting.multiplayerSetting != null)
            gameTimer.setGameType(MultiplayerSetting.multiplayerSetting.getGameType());
        else
            gameTimer.setGameType("auto");

        print("Started.....");
    }

    private void resetRobot()
    {
        if (PhotonNetwork.IsConnected)
        {
            var list = GameObject.FindGameObjectsWithTag("Player");
            for (int x = 0; x < list.Length; x++)
            {
                if (list[x].GetComponent<PhotonView>().IsMine)
                {
                    robot = list[x];
                }
            }
            if (robot != null)
                robot.GetComponent<PhotonView>().RPC("resetBalls", RpcTarget.AllBuffered);
        }
        else
        {
            robot.GetComponent<RobotController>().resetBalls();
        }
        if (robot != null)
        {
            robot.transform.position = robot.GetComponent<RobotController>().getStartPosition().position;
            robot.transform.rotation = robot.GetComponent<RobotController>().getStartPosition().rotation;
        }
    }

    public void resetField()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!currentGameStart && PhotonNetwork.IsMasterClient)
                GetComponent<PhotonView>().RPC("resetFieldHelper", RpcTarget.All);
            else if(currentGameStart)
                GetComponent<PhotonView>().RPC("resetFieldHelper", RpcTarget.All);
        }
        else
            resetFieldHelper();
    }

    [PunRPC]
    public void resetFieldHelper() 
    {
        print("resetting");
        if (PhotonNetwork.IsMasterClient)
        {
            string type = MultiplayerSetting.multiplayerSetting.getFieldSetup();
            resetRobot();
            ScoreKeeper._Instance.resetScore();
            if (setup != null)
            {
                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.Destroy(setup);
                }
                else
                {
                    Destroy(setup);
                }
            }

            int index;
            if (type == "A")
            {
                index = 0;
            }
            else if (type == "B")
            {
                index = 1;
            }
            else if (type == "C")
            {
                index = 2;
            }
            else
            {
                Random rnd = new Random();
                index = rnd.Next(3);
            }

            if (index == 0)
            {
                gameTimer.setGameSetup("A");
                type = "A";
            }
            else if (index == 1)
            {
                gameTimer.setGameSetup("B");
                type = "B";
            }
            else if (index == 2)
            {
                gameTimer.setGameSetup("C");
                type = "C";
            }

            emptyField();

            if (!PhotonNetwork.IsConnected)
            {
            }
            else
            {
                setup = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "DynamicField-" + type), new Vector3(0, 0.0f, 0), Quaternion.identity, 0);
                PhotonNetwork.SendAllOutgoingCommands();
            }
            for (int x = 0; x < setup.GetComponentsInChildren<Rigidbody>().Length; x++)
            {
                if (setup.GetComponentsInChildren<Rigidbody>()[x].tag == "Wobble")
                {
                    setup.GetComponentsInChildren<Rigidbody>()[x].centerOfMass = new Vector3(0, -0.9f, 0);
                }
            }
        }
        else if(!PhotonNetwork.IsConnected)
        {
            resetRobot();
            ScoreKeeper._Instance.resetScore();
            if (setup != null)
            {
                Destroy(setup);
            }

            Random rnd = new Random();
            int index = rnd.Next(3);

            if (index == 0)
            {
                gameTimer.setGameSetup("A");
            }
            else if (index == 1)
            {
                gameTimer.setGameSetup("B");
            }
            else if (index == 2)
            {
                gameTimer.setGameSetup("C");
            }
            emptyField();
            setup = (GameObject)Instantiate(setupPrefab[index], new Vector3(0, 0.0f, 0), Quaternion.identity);
        }
        else//for non-master clients
        {
            resetRobot();
        }
    }

    public void emptyField()
    {
        if(PhotonNetwork.IsConnected)
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("OutsideRing");

            foreach (GameObject a in gos)
            {
                if(a.GetPhotonView().IsMine)
                    PhotonNetwork.Destroy(a.GetPhotonView());
            }

            gos = GameObject.FindGameObjectsWithTag("BlueWobble");

            foreach (GameObject a in gos)
            {
                if (a.GetPhotonView().IsMine)
                    PhotonNetwork.Destroy(a.GetPhotonView());
            }

            gos = GameObject.FindGameObjectsWithTag("RedWobble");

            foreach (GameObject a in gos)
            {
                if (a.GetPhotonView().IsMine)
                    PhotonNetwork.Destroy(a.GetPhotonView());
            }
        }
        else
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("OutsideRing");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("BlueWobble");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("RedWobble");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }
        }
    }
    public void buttonStartGame()
    {
        if (!PhotonNetwork.IsConnected)
            startGame();
        else if (PhotonNetwork.IsMasterClient)
            GetComponent<PhotonView>().RPC("startGame", RpcTarget.All);
    }

    public void buttonStopGame()
    {
        if (!PhotonNetwork.IsConnected)
            stopGame();
        else if (PhotonNetwork.IsMasterClient)
            GetComponent<PhotonView>().RPC("stopGame", RpcTarget.All);
    }

    [PunRPC]
    public void startGame()
    {
        if (!currentGameStart)
        {
            resetRobot();
            resetField();
            currentGameStart = true;
            gameTimer.startGame();
        }
    }

    [PunRPC]
    public void stopGame()
    {
        if (currentGameStart)
        {
            currentGameStart = false;
            gameTimer.stopGame();
        }
    }

    public void exitGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (intake == null)
        {
            if (PhotonNetwork.IsConnected)
            {
                var list = GameObject.FindGameObjectsWithTag("Player");
                for (int x = 0; x < list.Length; x++)
                {
                    if (list[x].GetComponent<PhotonView>().IsMine)
                    {
                        intake = list[x].GetComponentInChildren<IntakeControl>();
                    }
                }
            }
        }
    }
}
