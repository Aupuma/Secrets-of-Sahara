using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public string[] scenes;
    private int sceneIndex;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        sceneIndex = 0;
    }

    public void LoadNextScene()
    {
        NetworkManager.singleton.ServerChangeScene(scenes[sceneIndex]);
        sceneIndex++;
    }

}
