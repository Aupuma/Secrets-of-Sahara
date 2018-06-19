using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public float totalTime;
    public float intervalToShowTime;
    [SyncVar] private float timeLeft;

    public string[] scenes;
    private int sceneIndex;

    public PlayerConnectionObject POVConnection;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        sceneIndex = 0;
    }

    public void LoadNextScene()
    {
        NetworkManager.singleton.ServerChangeScene(scenes[sceneIndex]);
        sceneIndex++;
    }

    private void Update()
    {
        if (isServer)
        {
            
        }
    }

    [ClientRpc]
    void RpcShowTimeLeft()
    {

    }

}
