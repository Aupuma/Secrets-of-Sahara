using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class EnemyPathInfo
{
    public Transform spawnPoint;
    public Transform objective;
}

public class EnemyManager : NetworkBehaviour {

    public static EnemyManager instance;

    private int lastEnemySpawned = -1;
    private bool ready = false;

    [Header("Spawn parameters")]//-------------------------------------------------
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    [Range(0f, 1f)] public float superEnemyProbability;
    private float currentTimeBetweenSpawns;
    private float lastSpawnTime = 0;

    private int enemyToDestroy = -1;
    private int[] selectionNumbers;
    private Queue<Enemy> enemyQueue; //Utilizamos una cola por si se ampliase a secuencias de enemigos en el futuro
    public EnemyType currentEnemy;

    [Header("References")]//-------------------------------------------------------
    public EnemyPathInfo[] enemyPaths;
    public Enemy[] normalEnemies;
    public Enemy superEnemy;
    public GameObject[] traps;
    public GameObject[] symbolTextures;
    public GameObject sceneCamera;
    public Slider uiProgressBar;

    [Header("Score parameters")]//-------------------------------------------------
    public int pointsToWin = 10;
    public int pointsCorrect = 1;
    public int pointsIncorrect = -1;
    public int pointsSuperEnemyFailed = -5;

    [SyncVar(hook = "OnScoreChanged")]
    public int pointsScored;

    //-----------------------------------------------------------------------------

    public override void OnStartServer()
    {
        NetDiscovery.instance.StartAsServer();
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {

        if (isServer)
        {
            currentTimeBetweenSpawns = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
            selectionNumbers = new int[] { 0, 1, 2, 3 };
            enemyQueue = new Queue<Enemy>();
            //sceneCamera.SetActive(true);
        }
        else
        {
            Invoke("StartSpawning", 1f);
        }
    }
	
    void StartSpawning()
    {
        POVPlayerInteractions.instance.connection.CmdStartSpawningEnemies();
    }

	// Update is called once per frame
	void Update () {
        if (isServer) //SPAWNEAMOS ENEMIGOS PARA JUGADOR AR CADA X SEGUNDOS
        {
            if (ready && Time.unscaledTime - lastSpawnTime >= currentTimeBetweenSpawns)
            {
                currentTimeBetweenSpawns = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
                lastSpawnTime = Time.unscaledTime;
                SpawnEnemies();
            }
        }
        else if (Input.GetMouseButtonDown(0)) //DETECTAMOS SI JUGADOR POV PULSA BOTÓN DEL PANEL
        {
            RaycastHit hit = new RaycastHit();
            // Construct a ray from the current touch coordinates
            //ESTO HAY QUE ARREGLARLO
            Debug.Log(POVPlayerInteractions.instance.povCamera);
            Ray ray = POVPlayerInteractions.instance.povCamera.ScreenPointToRay(Input.mousePosition);
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

    #region ENEMY SPAWNING

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

    //REDISTRIBUYE EL ARRAY DE MANERA ALEATORIA
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

    private void ChangeEnemyToDestroy()
    {
        if (enemyQueue.Count > 0) enemyQueue.Dequeue();
        CmdFadeOutSymbol();

        //NUEVO ENEMIGO A DESTRUIR, DISTINTO AL ANTERIOR
        int rand = enemyToDestroy;
        while (rand == enemyToDestroy)
        {
            rand = UnityEngine.Random.Range(0, normalEnemies.Length);
        }
        enemyToDestroy = rand;

        //CANBIAMOS EL TIPO DE ENEMIGO Y ENCENDEMOS SU LUZ
        currentEnemy = normalEnemies[enemyToDestroy].type;
        CmdFadeInSymbol();
        Invoke("AssignNewEnemy", 1f);
    } 

    private void AssignNewEnemy()
    {
        enemyQueue.Enqueue(normalEnemies[enemyToDestroy]);
    }

    #endregion //ENEMY SPAWNING

    #region SCORE METHODS

    public void OnGestureUsedInEnemy(Enemy enemyToCompare)
    {
        Enemy firstEnemyInQueue = enemyQueue.Peek();
        if (enemyToCompare.type == firstEnemyInQueue.type)
        {
            //Hemos usado el símbolo en el enemigo correcto, sumamos puntuación
            if (pointsScored + pointsCorrect >= pointsToWin) pointsScored = pointsToWin;
            else
            {
                pointsScored += pointsCorrect;
                ChangeEnemyToDestroy();
            }
        }
        else
        {
            //Hemos usado el símbolo en el enemigo incorrecto, restamos puntuación
            if (pointsScored + pointsIncorrect < 0) pointsScored = 0;
            else pointsScored += pointsIncorrect;
        }
    }

    public void OnEnemyFinishedPath(Enemy enemyToCompare)
    {
        Enemy firstEnemyInQueue = enemyQueue.Peek();
        if (enemyToCompare.type == EnemyType.SUPER)
        {
            //Si el superenemigo llega al final restamos muchos puntos
            if (pointsScored + pointsSuperEnemyFailed < 0) pointsScored = 0;
            else pointsScored += pointsSuperEnemyFailed;
        }
    }

    public void OnScoreChanged(int newScore)
    {
        if (isServer)
        {
            if (newScore == pointsToWin) Debug.Log("gameFinished");
        }
        else
        {
            pointsScored = newScore;
            uiProgressBar.value = pointsScored;
        }
    }

    #endregion //SCORE METHODS

    #region NETWORK METHODS

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
            Debug.Log("Fading in");
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
            Debug.Log("Fading out");
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

    [ClientRpc]
    public void RpcStartSpawningEnemies()
    {
        if (isServer)
        {
            ready = true;
            ChangeEnemyToDestroy();
        }
    } 

    #endregion //NETWORK METHODS
}
