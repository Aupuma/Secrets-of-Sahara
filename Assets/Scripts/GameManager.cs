using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public string[] scenes;
    private int sceneIndex;

    public PlayerConnectionObject POVPlayerConnection;


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

}
