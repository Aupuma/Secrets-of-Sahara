using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class NetworkClientUI : MonoBehaviour {

    NetworkClient client;

    private void OnGUI()
    {
        string ipaddress = Network.player.ipAddress;
        GUI.Box(new Rect(18, Screen.height - 50, 100, 50), ipaddress);
        GUI.Label(new Rect(20, Screen.height - 30, 100, 20), "Status:" + client.isConnected);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
