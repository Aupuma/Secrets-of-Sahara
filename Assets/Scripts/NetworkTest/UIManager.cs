using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public Text statusText;
    public GameObject hostButton;
    public GameObject clientButton;
    public PlayerConnection connection;

    public int clicksToResetTargets;
    public int clicksMade;

    private void Start()
    {
        instance = this;
        clicksMade = 0;
    }

    public void StartServerButtonClicked()
    {
        hostButton.SetActive(false);
        clientButton.SetActive(false);
        statusText.text = "Status: Hosting";
        NetDiscovery.instance.StartAsServer();
        NetworkManager.singleton.StartHost();
    }

    public void StartClientButtonClicked()
    {
        hostButton.SetActive(false);
        clientButton.SetActive(false);
        statusText.text = "Status: Looking for server";
        NetDiscovery.instance.StartAsClient();
    }

    public void ResetTargetsButtonClicked()
    {
        clicksMade++;
        if(clicksMade == clicksToResetTargets)
        {
            connection.CmdResetTargets();
            clicksMade = 0;
        }
    }

    public void ConnectionFound()
    {
        statusText.text = "Status: Connected";
    }

    // Update is called once per frame
    void Update () {
		
	}
}
