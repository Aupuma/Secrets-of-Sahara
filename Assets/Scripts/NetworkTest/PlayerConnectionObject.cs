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
    [SyncVar (hook = "OnPlayerNameChanged")]
    public string PlayerName = "Anonymous";

    public override void OnStartClient()
    {
        /*
        if (isLocalPlayer == false)
        {
            Camera cam = FindObjectOfType<Camera>();
            if (cam != null) cam.enabled = false;
            return;
        }*/
    }

    void Start () {
        //Es éste mi PlayerObject local?
        if (isLocalPlayer == false)
        {
            Camera cam = FindObjectOfType<Camera>();
            if (cam != null) cam.enabled = false;
            return;
        }

        //Instantiate() solo crea un objeto en el ORDENADOR LOCAL
        //Incluso si tiene un NetworkIdentity no existirá en 
        //la red (y por lo tanto en ningún otro cliente) 
        //A NO SER QUE NetworkServer.Spawn() se llame en este objeto

        //Instantiate(PlayerUnitPrefab);

        //Decirle al servidor que spawnée nuestra unidad
        if (!isServer) Instantiate(ARPlayerCamera);
        else CmdSpawnMyUnit();
    }

    void Update () {
        if(isLocalPlayer == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdSpawnMyUnit();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string n = "Quill" + Random.Range(1, 100);

            CmdChangePlayerName(n);
        }
	}

    void OnPlayerNameChanged(string newName)
    {
        //WARNING: Si se usa un hook en una SyncVar, el valor local
        //NO se actualiza automáticamente
        PlayerName = newName;
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

    [Command]
    void CmdChangePlayerName(string n)
    {
        //Quizás queramos comprobar si el nombre no tiene ninguna palabra prohibida?
        //Si hay una palabra prohibida, ignoramos la petición y no hacemos nada?
        //   ...o seguimos llamando a la Rpc pero con el nombre original?

        PlayerName = n;

        //Cuéntale a todos los clientes cuál es el nombre actual del player
        RpcChangePlayerName(PlayerName);
    }

    //-------------------------------------RPC
    //RPCs son funciones especiales que SOLO se ejecutan en los clientes

    [ClientRpc]
    void RpcChangePlayerName(string n)
    {

    }
}
