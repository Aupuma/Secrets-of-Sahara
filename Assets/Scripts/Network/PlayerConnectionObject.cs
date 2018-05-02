using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

    public GameObject PlayerUnitPrefab;
    public GameObject ARPlayerCamera;
    private GameObject playerObject;

    void Start () {
        //Es éste mi PlayerObject local?
        if (isLocalPlayer == false)
        {
            return;
        }

        if(!isServer) //Soy el jugador en primera persona
        {
            CmdSpawnMyUnit();
            CmdSetUpFPObjects();
        }
        //FindObjectOfType<GameManager>().connection = this;
    }

    //--------------------------------------COMMANDS
    //Commandos son funciones especiales que SOLO se ejecutan en el servidor

    [Command]
    void CmdSpawnMyUnit()
    {
        playerObject = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(playerObject,connectionToClient);
    }

    [Command]
    void CmdSetUpFPObjects()
    {
    }

    //-------------------------------------RPC
    //RPCs son funciones especiales que SOLO se ejecutan en los clientes




    //---------------------------------------
}
