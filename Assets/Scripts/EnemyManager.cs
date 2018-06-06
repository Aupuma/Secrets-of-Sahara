﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[System.Serializable]
public class EnemyPathInfo
{
    public Transform spawnPoint;
    public Transform objective;
}

public class EnemyManager : NetworkBehaviour {

    public static EnemyManager instance;

    private int lastEnemySpawned = -1;
    private bool ready = true;

    [Header("Spawn parameters")]//-------------------------------------------------
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    [Range(0f, 1f)] public float superEnemyProbability;
    private float currentTimeBetweenSpawns;
    private float lastSpawnTime = 0;

    [Header("References")]//-------------------------------------------------------
    public EnemyPathInfo[] enemyPaths;
    public Enemy[] normalEnemies;
    public Enemy superEnemy;
    public GameObject[] traps;
    public GameObject[] symbolTextures;

    [Header("Score parameters")]//-------------------------------------------------
    public int pointsToWin = 10;
    public int pointsCorrect = 1;
    public int pointsIncorrect = -1;
    public int pointsSuperEnemyFailed = -5;
    [SyncVar] public int pointsScored;

    private int enemyToDestroy = -1;
    private int[] selectionNumbers;
    private Queue<Enemy> enemyQueue; //Utilizamos una cola por si se ampliase a secuencias de enemigos en el futuro
    public EnemyType currentEnemy;

    //-----------------------------------------------------------------------------

    public override void OnStartServer()
    {
        NetDiscovery.instance.StartAsServer();
    }
    
    // Use this for initialization
    void Start () {
        instance = this;
        currentTimeBetweenSpawns = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
        selectionNumbers = new int[] { 0, 1, 2, 3 };
        enemyQueue = new Queue<Enemy>();
        ChangeEnemyToDestroy();
    }
	
	// Update is called once per frame
	void Update () {
        if (isServer)
        {
            if (ready && Time.unscaledTime - lastSpawnTime >= currentTimeBetweenSpawns)
            {
                currentTimeBetweenSpawns = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
                lastSpawnTime = Time.unscaledTime;
                SpawnEnemies();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "TrapButton")
                {
                    Debug.Log("Button hit");
                    hit.collider.gameObject.GetComponent<Animator>().SetTrigger("Pressed");
                    POVPlayerInteractions.instance.connection.CmdRemoteTrapCall();
                }
            }
        }
	}

    private void SpawnEnemies()
    {
        ReshuffleArray(selectionNumbers);

        //Si el número aleatorio es menor que la probabilidad spawneamos el super enemigo
        float prob = UnityEngine.Random.Range(0f, 1f);
        int superEnemyPath = -1;

        //Camino aleatorio para el super enemigo en caso de que se spawnee
        if (prob < superEnemyProbability)
            superEnemyPath = UnityEngine.Random.Range(0, enemyPaths.Length);

        //Spawneamos en cada carril un enemigo distinto
        for (int i = 0; i < enemyPaths.Length; i++)
        {
            Enemy enemy = null;
            if (i == superEnemyPath)
            {
                enemy = Instantiate(superEnemy, enemyPaths[i].spawnPoint.position, Quaternion.identity);
            }
            else
            {
                enemy = Instantiate(normalEnemies[selectionNumbers[i]], enemyPaths[i].spawnPoint.position, Quaternion.identity);
            }

            enemy.GetComponent<NavMeshAgent>().SetDestination(enemyPaths[i].objective.position);
        }
    }

    private void ChangeEnemyToDestroy()
    {
        if(enemyQueue.Count>0) enemyQueue.Dequeue(); //Desencolamos el enemigo antiguo y apagaríamos su símbolo

        int rand = enemyToDestroy;
        while(rand == enemyToDestroy)
        {
            rand = UnityEngine.Random.Range(0, normalEnemies.Length);
        }
        enemyToDestroy = rand;

        currentEnemy = normalEnemies[enemyToDestroy].type;
        CmdFadeInSymbol();
        enemyQueue.Enqueue(normalEnemies[enemyToDestroy]);
    }

    public void OnGestureUsedInEnemy(Enemy enemyToCompare)
    {
        Enemy firstEnemyInQueue = enemyQueue.Peek();
        if (enemyToCompare.type == firstEnemyInQueue.type)
        {
            //Hemos usado el símbolo en el enemigo correcto, sumamos puntuación
            pointsScored += pointsCorrect;
            CmdFadeOutSymbol();
            Invoke("ChangeEnemyToDestroy", 3f);
        }
        else
        {
            //Hemos usado el símbolo en el enemigo incorrecto, restamos puntuación
            pointsScored += pointsIncorrect;
        }
    }

    public void OnEnemyFinishedPath(Enemy enemyToCompare)
    {
        Enemy firstEnemyInQueue = enemyQueue.Peek();
        if (enemyToCompare.type == EnemyType.SUPER)
        {
            //Si el superenemigo llega al final restamos muchos puntos
            pointsScored += pointsSuperEnemyFailed;
        }
    }

    void ReshuffleArray(int[] numbers)
    {
        for (int t = 0; t < numbers.Length; t++)
        {
            int tmp = numbers[t];
            int r = UnityEngine.Random.Range(t, numbers.Length);
            numbers[t] = numbers[r];
            numbers[r] = tmp;
        }
    }

    [Command]
    public void CmdFadeInSymbol()
    {
        RpcFadeInSymbol(enemyToDestroy);
    }

    [ClientRpc]
    public void RpcFadeInSymbol(int index)
    {
        if (!isServer)
        {
            symbolTextures[index].GetComponent<Animator>().SetTrigger("fadeIn");
        }
    }

    [Command]
    public void CmdFadeOutSymbol()
    {
        RpcFadeOutSymbol(enemyToDestroy);
    }

    [ClientRpc]
    public void RpcFadeOutSymbol(int index)
    {
        if (!isServer)
        {
            symbolTextures[index].GetComponent<Animator>().SetTrigger("fadeOut");
        }
    }

    [ClientRpc]
    public void RpcTrapsOnOff()
    {
        if (isServer)
        {
            Debug.Log("Moving traps");
            foreach (var t in traps)
            {
                t.GetComponent<Animator>().SetTrigger("Move");
            }
        }
    }
}
