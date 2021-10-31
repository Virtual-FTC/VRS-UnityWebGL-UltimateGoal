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

public class FieldManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private ScoreKeeper scoreKeeper;
    private IntakeControl intake;
    private CameraPosition cam;

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

        cam = FindObjectOfType<CameraPosition>();
        gameTimer = FindObjectOfType<GameTimer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
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

            cam.switchCamera(MultiplayerSetting.multiplayerSetting.getCamSetup());
            gameTimer.setGameType(MultiplayerSetting.multiplayerSetting.getGameType());
        }
        else
        {
            intake = GameObject.Find("Intake").GetComponent<IntakeControl>();
        }



        ResetField();


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
        }
        intake.resetBalls();

        robot.transform.position = robot.GetComponent<RobotController>().getStartPosition().position;
        robot.transform.rotation = robot.GetComponent<RobotController>().getStartPosition().rotation;
    }

    public void ResetField()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            string type = MultiplayerSetting.multiplayerSetting.getFieldSetup();
            resetRobot();
            scoreKeeper.resetScore();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Destroy(setup);
            }
            else
            {
                Destroy(setup);
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

            GameObject[] gos = GameObject.FindGameObjectsWithTag("Cube");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("Duck");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("BlueHub");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("RedHub");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            gos = GameObject.FindGameObjectsWithTag("JointHub");

            foreach (GameObject a in gos)
            {
                Destroy(a);
            }

            if (!PhotonNetwork.IsConnected)
            {
                setup = (GameObject)Instantiate(setupPrefab[index], new Vector3(0, 0.0f, 0), Quaternion.identity);
            }
            else
            {
                setup = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "DynamicFieldFreight"), new Vector3(0, 0.0f, 0), Quaternion.identity, 0);
            }
        }
        else
        {
            resetRobot();
            scoreKeeper.resetScore();
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
            currentGameStart = true;
            resetRobot();
            ResetField();
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
