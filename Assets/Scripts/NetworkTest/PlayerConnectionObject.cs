using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

    public GameObject PlayerUnitPrefab;
    public GameObject ARPlayerCamera;

    //[SyncVar]
    //public static bool playerExists = false;

    //SyncVars son variables en las que si su valor cambia en el SERVIDOR, 
    //todos los clientes son informados automáticamente del nuevo valor

    void Start () {
        //Es éste mi PlayerObject local?
        if (isLocalPlayer == false)
        {
            return;
        }

        //Instantiate() solo crea un objeto en el ORDENADOR LOCAL
        //Incluso si tiene un NetworkIdentity no existirá en 
        //la red (y por lo tanto en ningún otro cliente) 
        //A NO SER QUE NetworkServer.Spawn() se llame en este objeto

        //Instantiate(PlayerUnitPrefab);

        //Decirle al servidor que spawnée nuestra unidad
        if (isServer) //Soy el host, jugador con camara AR
        {
            Instantiate(ARPlayerCamera);
        }
        else
        {
            CmdSpawnMyUnit();
        }
    }

    //--------------------------------------COMMANDS
    //Commandos son funciones especiales que SOLO se ejecutan en el servidor

    [Command]
    void CmdSpawnMyUnit()
    {
        //Tenemos la garantía de estar en el servidor
        GameObject go = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);

        //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);

        //Ahora que el objeto existe en el servidor, propagarlo a todos
        //los clientes (y conectarlo a NetworkIdentity)
        NetworkServer.SpawnWithClientAuthority(go,connectionToClient);

       // playerExists = true;
    }


    //-------------------------------------RPC
    //RPCs son funciones especiales que SOLO se ejecutan en los clientes

}
