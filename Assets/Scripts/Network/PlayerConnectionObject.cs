using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

    public GameObject PlayerUnitPrefab;
    public GameObject ARPlayerCamera;

    public GameObject gameManager;

    void Start () {
        DontDestroyOnLoad(gameObject);
        if (isServer) //Servidor, jugador PAR
        {
            if (GameManager.instance == null) SpawnGameManager();
            //if(PARCamera.instance == null) Instantiate(ARPlayerCamera);
        }
        else //No es servidor, es el jugador POV
        {
            GameManager.instance.POVConnection = this;
            if (MazeManager.instance != null) CmdSpawnPOVPlayerObj(); //TEMPORAL
        }
    }

    void SpawnGameManager()
    {
        GameObject gm = Instantiate(gameManager, this.transform.position, Quaternion.identity);
        NetworkServer.Spawn(gm);
    }

    //---------------COMMANDS---------------------------------------------
    //Commandos son funciones especiales que SOLO se ejecutan en el servidor

    //---------MAZE
    [Command]
    public void CmdSpawnPOVPlayerObj()
    {
        GameObject playerObject = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(playerObject,connectionToClient);
        MazeManager.instance.EnableFirstTraps();
    }

    //---------GESTURES ENEMIES PUZZLE
    [Command]
    public void CmdStartSpawningEnemies()
    {
        EnemyManager.instance.StartSpawningEnemies();
    }

    [Command]
    public void CmdRemoteTrapCall(int index)
    {
        EnemyManager.instance.TrapsOnOff(index);
    }

    //---------PERSPECTIVE PUZZLE
    [Command]
    public void CmdRotationCall(int index)
    {
        PerspectivePuzzleManager.instance.RpcRotateElements(index);
    }
    //---------------------------------------
}
