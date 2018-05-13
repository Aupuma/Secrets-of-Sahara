using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyPathInfo
{
    public Transform spawnPoint;
    public Transform objective;
    public GameObject enemyPrefab;
}

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;

    private int lastEnemySpawned = -1;
    private bool ready = true;

    //Time settings
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    private float currentTimeBetweenSpawns;
    private float lastSpawnTime = 0;

    public EnemyPathInfo[] enemyPaths;
    
    [Range(0f, 1f)]
    public float superEnemyProbability;
    public GameObject superEnemyPrefab;

    // Use this for initialization
    void Start () {
        instance = this;
        currentTimeBetweenSpawns = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
        NetDiscovery.instance.StartAsServer();
    }
	
	// Update is called once per frame
	void Update () {
        if(ready && Time.unscaledTime - lastSpawnTime >= currentTimeBetweenSpawns)
        {
            currentTimeBetweenSpawns = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
            lastSpawnTime = Time.unscaledTime;
            SpawnEnemy();
        }
	}

    private void SpawnEnemy()
    {
        int rand = lastEnemySpawned;
        while (rand == lastEnemySpawned)
        {
            rand = UnityEngine.Random.Range(0, enemyPaths.Length); //Creamos nº random para elegir enemigo
        }
        lastEnemySpawned = rand;

        float prob = UnityEngine.Random.Range(0f, 1f);
        Debug.Log(prob);
        if (prob < superEnemyProbability) //Si el número aleatorio es menor que la probabilidad spawneamos el super enemigo
        {
            GameObject enemy = Instantiate(superEnemyPrefab, enemyPaths[rand].spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<NavMeshAgent>().SetDestination(enemyPaths[rand].objective.position);
        }
        else //Si no spawneamos enemigo normal
        {
            GameObject enemy = Instantiate(enemyPaths[rand].enemyPrefab, enemyPaths[rand].spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<NavMeshAgent>().SetDestination(enemyPaths[rand].objective.position);
        }
    }
}
