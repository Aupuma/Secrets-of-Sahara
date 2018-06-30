using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public GameObject menuScreen;
    public GameObject loadingScreen;
    public string arSetupScene;
    private Animator animator;

    #region SINGLETON
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartARButtonClicked()
    {
        animator.SetTrigger("ARmode");
    }

    public void StartARMode()
    {
#if UNITY_EDITOR
        NetworkManager.singleton.StartHost();

#elif  UNITY_STANDALONE
        NetworkManager.singleton.StartHost();

#elif UNITY_ANDROID
        SceneManager.LoadScene(arSetupScene);

#elif UNITY_IOS
        SceneManager.LoadScene(arSetupScene);
#endif
    }

    public void StartPOVButtonClicked()
    {
        //Esto se hará con transición
        //Aqui se puede empezar a cargar la escena también
        loadingScreen.SetActive(true);
        menuScreen.SetActive(false);
        NetworkTransport.Init();
        NetDiscovery.instance.Initialize();
        NetDiscovery.instance.StartAsClient();
    }

    public void BackFromLoadingScreenClicked()
    {
        loadingScreen.SetActive(false);
        menuScreen.SetActive(true);
        NetDiscovery.instance.StopBroadcast();
        NetworkTransport.Shutdown();
    }

    public void PlayFadeToPOVMode()
    {
        animator.SetTrigger("POVmode");
    }

    public void StartPOVMode()
    {
        NetworkManager.singleton.StartClient();
    }

    public void ExitFromGame()
    {
        Application.Quit();
    }
}
