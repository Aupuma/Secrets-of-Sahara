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

        //Es éste mi PlayerObject local?
        /*
        if(isServer) //Soy el jugador en primera persona
        {
            if(GameManager.instance == null)
            {
                CmdSpawnGameManager();
            }
        }
        */
        if (!isServer && GameManager.instance == null)
        {
            CmdSpawnGameManager();
        }
        //if (!isServer) CmdSpawnPOVPlayerObj();
        //FindObjectOfType<GameManager>().connection = this;
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
    void CmdSpawnPOVPlayerObj()
    {
        GameObject playerObject = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(playerObject,connectionToClient);
        RpcAssignConnectionToPOVPlayer(playerObject);
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

    //-------------------------------------RPC
    //RPCs son funciones especiales que SOLO se ejecutan en los clientes
    [ClientRpc]
    void RpcAssignConnectionToPOVPlayer(GameObject playerObject)
    {
        if (!isServer) playerObject.GetComponent<POVPlayerInteractions>().connection = this;
    }
    //---------------------------------------
}
