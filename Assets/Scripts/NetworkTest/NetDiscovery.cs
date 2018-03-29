using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetDiscovery : NetworkDiscovery{

    public static NetDiscovery instance;
    public bool connected = false;

    /*
    public NetDiscovery()
    {
        broadcastPort = 8080;
        broadcastInterval = 1000;
        broadcastKey = 2222;
        broadcastVersion = 1;
        broadcastSubVersion = 1;
        useNetworkManager = true;
        showGUI = true;
    }
*/
    // Use this for initialization
    void Start () {
        instance = this;
        Initialize();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (!connected)
        {
            connected = true;
            UIManager.instance.ConnectionFound();
            NetworkManager.singleton.networkAddress = fromAddress;
            NetworkManager.singleton.StartClient();
        }
        Debug.Log("Broadcast: " + fromAddress);
    }
}
