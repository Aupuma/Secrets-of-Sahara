using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetDiscovery : NetworkDiscovery{

    public static NetDiscovery instance;
    public bool connected = false;

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
            Debug.Log("Broadcast: " + fromAddress);
            NetworkManager.singleton.networkAddress = fromAddress;
            UIManager.instance.PlayFadeToPOVMode();
            StopBroadcast();
        }
    }
}
