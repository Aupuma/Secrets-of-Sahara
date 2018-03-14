using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityARInterface;

public class LoadGameScene : ARBase {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadGame()
    {
        CustomNetworkDiscovery.Instance.StartBroadcasting();
        NetworkManager.singleton.StartHost();
        //SceneManager.LoadScene(3);
    }
}
