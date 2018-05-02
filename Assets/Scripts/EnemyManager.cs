using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;

    private List<Enemy> enemyList;
    private int lastEnemySpawned = -1;
    public Enemy[] enemyPrefabs;
    public Transform[] enemySpawnPos;
    public GameObject player;
    private bool ready = false;

    //Time settings
    public float timeBetweenSpawns;
    private float lastSpawnTime = 0;

	// Use this for initialization
	void Start () {
        instance = this;
        enemyList = new List<Enemy>();
	}
	
	// Update is called once per frame
	void Update () {
        if(ready && Time.unscaledTime - lastSpawnTime >= timeBetweenSpawns)
        {
            lastSpawnTime = Time.unscaledTime;
            SpawnEnemy();
        }
	}

    public void SetPlayerObj(GameObject gameObject)
    {
        player = gameObject;
        ready = true;
    }

    private void SpawnEnemy()
    {
        int rand = lastEnemySpawned;
        while(rand == lastEnemySpawned)
        {
            rand = UnityEngine.Random.Range(0, enemyPrefabs.Length); //Creamos nº random para elegir enemigo
        }
        lastEnemySpawned = rand;

        int j = UnityEngine.Random.Range(0, enemySpawnPos.Length);

        Enemy newEnemy = Instantiate(enemyPrefabs[rand], enemySpawnPos[j].position, Quaternion.identity) as Enemy;
        newEnemy.player = player;
        enemyList.Add(newEnemy);
    }

    public void DestroyEnemiesOfGesture(string type)
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            //Eliminamos puntos generados por el trazo
            if (enemyList[i].gestureType == type)
            {
                GameObject enemyToDestroy = enemyList[i].gameObject;
                enemyList.Remove(enemyList[i]);
                Destroy(enemyToDestroy);
            }
        }
    }
}
