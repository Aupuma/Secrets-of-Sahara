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
        if (!isServer && GameManager.instance == null)
        {
            CmdSpawnGameManager();
        }
    }

    //--------------------------------------COMMANDS
    //Commandos son funciones especiales que SOLO se ejecutan en el servidor

    [Command]
    void CmdSpawnGameManager()
    {
        GameObject gm = Instantiate(gameManager, this.transform.position, Quaternion.identity);
        NetworkServer.Spawn(gm);
        RpcAssignConnectionToGM();
    }

    [ClientRpc]
    void RpcAssignConnectionToGM()
    {
        if (!isServer)
        {
            GameManager.instance.POVPlayerConnection = this;
        }
    }

    //---------MAZE----------------------
    [Command]
    public void CmdSpawnPOVPlayerObj()
    {
        GameObject playerObject = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(playerObject,connectionToClient);
        MazeManager.instance.EnableFirstTraps();
    }

    //---------GESTURES ENEMIES PUZZLE----------------------
    [Command]
    public void CmdRemoteTrapCall(int index)
    {
        EnemyManager.instance.TrapsOnOff(index);
    }

    //---------PERSPECTIVE PUZZLE----------------------
    [Command]
    public void CmdRotationCall(int index)
    {
        RotatingPuzzleManager.instance.RpcRotateElements(index);
    }
    //---------------------------------------
}
