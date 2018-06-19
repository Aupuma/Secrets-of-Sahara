using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    //Countdown timer
    [SyncVar] private float timeLeft;
    public int totalTime;
    public int intervalToShowTime;
    public GameObject timerObject;
    private Animator timerAnimator;
    private Text timerText;

    public string[] scenes;
    private int sceneIndex;

    public PlayerConnectionObject POVConnection;


    #region SINGLETON
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    } 
    #endregion

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        sceneIndex = 0;
        timeLeft = totalTime;
        timerAnimator = timerObject.GetComponent<Animator>();
        timerText = timerObject.GetComponent<Text>();

        timerAnimator.SetTrigger("InitialAnim");
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
            timeLeft -= Time.deltaTime;
            //if((int)timeLeft % 60 == )
        }

        string minSec = string.Format("{0}:{1:00}", (int)timeLeft / 60, (int)timeLeft % 60);
        timerText.text = minSec;
    }

    [ClientRpc]
    void RpcShowTimeLeft()
    {
        timerAnimator.SetTrigger("Show");
    }

}
