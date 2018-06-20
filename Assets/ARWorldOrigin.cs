using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ARWorldOrigin : MonoBehaviour {

    public static ARWorldOrigin instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        NetworkManager.singleton.StartHost();
    }
}
