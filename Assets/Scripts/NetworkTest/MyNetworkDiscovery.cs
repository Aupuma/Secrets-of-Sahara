using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery {



    void Start()
    {
        InitializeNetworkDiscovery();
    }

    public bool InitializeNetworkDiscovery()
    {
        return Initialize();
    }

    public void StartBroadcasting()
    {
        StartAsServer();
    }

    public void SetBroadcastData(string broadcastPayload)
    {
        broadcastData = broadcastPayload;
    }

    public void ReceiveBraodcast()
    {
        StartAsClient();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
