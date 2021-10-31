using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGameManager : MonoBehaviour
{
    [SerializeField] GameObject robot;
    [SerializeField] GameObject[] objectsToSpawnInWarehouse;
    [SerializeField] GameObject duck;
    [SerializeField] Transform[] blueTeamDuckSpawns, redTeamDuckSpawns;
    [SerializeField] Transform blueSpawn, redSpawn;
    [SerializeField] Transform blueItemSpawn, redItemSpawn;
    [SerializeField]Transform redCarouselDuckSpawn, blueCarouselDuckSpawn;

    

    List<GameObject> spawnedItems;

    bool gameStarted;

    void Start()
    {
        spawnedItems = new List<GameObject>();

        SpawnRobots();

    }

    void SpawnRobots()
    {
        Instantiate(robot, blueSpawn.position, Quaternion.identity);

    }

    void SpawnDucks()
    {
        int spawnIndex = Random.Range(0, 3);

        SpawnItem(duck, blueTeamDuckSpawns[spawnIndex].position);
        SpawnItem(duck, redCarouselDuckSpawn.position);
        SpawnItem(duck, redTeamDuckSpawns[spawnIndex].position );
        SpawnItem(duck, blueCarouselDuckSpawn.position);
    }

    void SpawnWarehouseItems()
    {
        for (int i = 0; i < objectsToSpawnInWarehouse.Length; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            SpawnItem(objectsToSpawnInWarehouse[i], blueItemSpawn.position + offset);
        }

        for (int i = 0; i < objectsToSpawnInWarehouse.Length; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            SpawnItem(objectsToSpawnInWarehouse[i], redItemSpawn.position + offset);
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;
        SpawnDucks();
        gameStarted = true;
    }

    public void EndGame()
    {
        gameStarted = false;
        foreach (GameObject obj in spawnedItems)
        {
            Destroy(obj);
        }
    }

    void SpawnItem(GameObject objToSpawn,Vector3 pos)
    {
        spawnedItems.Add(Instantiate(objToSpawn, pos, objToSpawn.transform.rotation));
    }
}
