using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGameManager : MonoBehaviour
{
    [SerializeField] GameObject robot;
    [SerializeField] GameObject xCube, lCube, mCube, hCube,duck,ball;
    [SerializeField] Transform[] blueTeamDuckSpawns, redTeamDuckSpawns;
    [SerializeField] Transform blueSpawn, redSpawn;
    // Start is called before the first frame update
    void Start()
    {
        SpawnRobots();
        SpawnDucks();
    }

    void SpawnRobots()
    {
        Instantiate(robot, blueSpawn.position, Quaternion.identity);

    }

    void SpawnDucks()
    {
        int spawnIndex = Random.Range(0, 3);

        Instantiate(duck, blueTeamDuckSpawns[spawnIndex].position + Vector3.up, Quaternion.identity);
        Instantiate(duck, redTeamDuckSpawns[spawnIndex].position + Vector3.up,Quaternion.identity);
    }
}
