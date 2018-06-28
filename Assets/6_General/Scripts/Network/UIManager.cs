using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    //public static UIManager instance;
    public bool isDebug = false;

    public GameObject menuScreen;
    public GameObject loadingScreen;

    public string arSetupScene;
    
    private void Start()
    {
        //instance = this;
    }

    public void StartServerButtonClicked()
    {
        if(isDebug) NetworkManager.singleton.StartHost();
        else SceneManager.LoadScene(arSetupScene);
    }

    public void StartClientButtonClicked()
    {
        //Esto se hará con transición
        //Aqui se puede empezar a cargar la escena también
        loadingScreen.SetActive(true);
        menuScreen.SetActive(false);
        NetDiscovery.instance.StartAsClient();
    }

    public void BackFromLoadingScreenClicked()
    {
        loadingScreen.SetActive(false);
        menuScreen.SetActive(true);
        NetDiscovery.instance.StopBroadcast();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
