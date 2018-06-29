using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    [Header("Countdown timer")]
    [SyncVar] private float secondsLeft;
    public int secsIntervalToShowTimer;
    public int totalMinutes;
    [SyncVar] private bool timerEnabled;
    public GameObject timerObject;
    private Animator timerAnimator;
    private Text timerText;

    [Header("References")]
    public string[] scenes;
    private int sceneIndex;
    public Animator faderAnimator;

    [HideInInspector]
    public PlayerConnectionObject POVConnection;

    #region SINGLETON
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public override void OnStartServer()
    {
        NetDiscovery.instance.StartAsServer();
    }

    private void Start()
    {
        gameObject.DontDestroyOnLoad();
        sceneIndex = 0;

        secondsLeft = totalMinutes * 60;
        timerAnimator = timerObject.GetComponent<Animator>();
        timerText = timerObject.GetComponent<Text>();
        timerEnabled = false;
        
    }

    private void Update()
    {
        if (timerEnabled) UpdateSecondsLeft();
    }

    public void LoadNextScene()
    {
        RpcOpenPOVWalls();
        NetworkManager.singleton.ServerChangeScene(scenes[sceneIndex]);
        sceneIndex++;
    }

    [ClientRpc]
    private void RpcOpenPOVWalls()
    {
        if(!isServer && POVRoom.instance!=null)
            POVRoom.instance.animator.SetTrigger("OpenWalls");
    }

    public void LoadDefeatScene()
    {
       // NetworkManager.singleton.ServerChangeScene(scenes[sceneIndex]);
    }

    #region SCREEN FADER

    public void FadeOnStart()
    {
        if (!isServer) faderAnimator.SetTrigger("InitialFade");
    }

    public void FadeDamage()
    {
        if (!isServer) faderAnimator.SetTrigger("Damaged");
    }

    public void FadeOnTeleport()
    {
        if(!isServer) faderAnimator.SetTrigger("ChangeScene");
    }

    public void FinishLoadingFade()
    {
        if (!isServer) faderAnimator.SetTrigger("SceneLoaded");
    }

    #endregion

    #region COUNTDOWN TIMER

    /// <summary>
    /// Si somos el servidor llamamos a la corrutina para que empiece con los intervalos
    /// y activamos la animación inicial del timer
    /// </summary>
    [ClientRpc]
    public void RpcShowInitialTimerAnim()
    {
        if (isServer) StartCoroutine(TimeIntervals());
        timerAnimator.SetTrigger("InitialAnim");
    }

    private void UpdateSecondsLeft()
    {
        if (isServer){
            secondsLeft -= Time.deltaTime; //El servidor actualiza la synvar del temporizador

            if (secondsLeft < 0)
            {
                secondsLeft = 0;
                timerEnabled = false;
                LoadDefeatScene();
            }
        }

        //Actualizamos el texto del timer
        string minSec = string.Format("{0}:{1:00}", (int)secondsLeft / 60, (int)secondsLeft % 60);
        timerText.text = minSec;
    }

    /// <summary>
    /// Inicialmente esperamos un segundo para empezar a contar los segundos
    /// y después llamamos a mostrar timer cada x segundos
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeIntervals()
    {
        yield return new WaitForSeconds(1f);
        timerEnabled = true;
        yield return null;

        while (timerEnabled)
        {
            yield return new WaitForSeconds(secsIntervalToShowTimer);
            RpcShowTimeLeft();
        }
    }

    /// <summary>
    /// Mostramos el timer en los dos jugadores
    /// </summary>
    [ClientRpc]
    void RpcShowTimeLeft()
    {
        timerAnimator.SetTrigger("Show");
    } 

    #endregion //COUNTDOWN TIMER

}
