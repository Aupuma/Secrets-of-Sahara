using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameOverCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("DestroyPersistenObjects", 0.25f);
	}
	
    private void DestroyPersistenObjects()
    {
        DontDestroyOnLoadManager.DestroyAll();
    }

    public void BackToMainMenuPressed()
    {
        NetDiscovery.instance.StopBroadcast();
        NetworkManager.singleton.StopHost();
    }

}
