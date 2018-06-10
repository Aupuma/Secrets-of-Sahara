using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

    public GameObject PlayerUnitPrefab;
    public GameObject ARPlayerCamera;

    private void Awake()
    {
        //if (!isServer) CmdSpawnPOVPlayerObj();
    }

    void Start () {
        DontDestroyOnLoad(gameObject);

        //Es éste mi PlayerObject local?
        if (isLocalPlayer == false)
        {
            return;
        }

        if(!isServer) //Soy el jugador en primera persona
        {
            CmdSpawnPOVPlayerObj();
        }
        //FindObjectOfType<GameManager>().connection = this;
    }

    //--------------------------------------COMMANDS
    //Commandos son funciones especiales que SOLO se ejecutan en el servidor

    //---------MAZE----------------------
    [Command]
    void CmdSpawnPOVPlayerObj()
    {
        GameObject playerObject = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(playerObject,connectionToClient);
        RpcAssignConnectionToPOVPlayer(playerObject);
        MazeManager.instance.CmdEnableFirstTraps();
    }

    //---------GESTURES ENEMIES PUZZLE----------------------
    [Command]
    public void CmdRemoteTrapCall()
    {
        EnemyManager.instance.RpcTrapsOnOff();
    }

    [Command]
    public void CmdStartSpawningEnemies()
    {
        EnemyManager.instance.RpcStartSpawningEnemies();
    }

    //---------PERSPECTIVE PUZZLE----------------------
    [Command]
    public void CmdRotationCall(int index)
    {
        RotatingPuzzleManager.instance.RpcRotateElements(index);
    }

    //-------------------------------------RPC
    //RPCs son funciones especiales que SOLO se ejecutan en los clientes
    [ClientRpc]
    void RpcAssignConnectionToPOVPlayer(GameObject playerObject)
    {
        if (!isServer) playerObject.GetComponent<POVPlayerInteractions>().connection = this;
    }
    //---------------------------------------
}
