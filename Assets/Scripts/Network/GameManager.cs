using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public PlayerConnectionObject connection;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        if (isServer)
        {
            NetDiscovery.instance.StartAsServer();
        }
        else
        {
            NetworkManager.singleton.ServerChangeScene("GameTestScene");
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
