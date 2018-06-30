using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public bool isDebug = false;

    public GameObject menuScreen;
    public GameObject loadingScreen;

    public string arSetupScene;

    public void StartServerButtonClicked()
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

    public void StartClientButtonClicked()
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

    // Update is called once per frame
    void Update () {
		
	}
}
