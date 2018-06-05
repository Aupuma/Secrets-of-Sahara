using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class WirePuzzleManager : NetworkBehaviour {

    public static WirePuzzleManager instance;

    [SyncVar(hook = "OnChangeIndex")]
    public int currentPuzzleIndex = 0;

    public GameObject[] panelLights;
    private bool[] lightsOn;

	void Start () {
        instance = this;
        lightsOn = new bool[panelLights.Length];
	}

    public override void OnStartServer()
    {
        NetDiscovery.instance.StartAsServer();
    }

    void OnChangeIndex(int index)
    {
        if(isServer) 
            if(index == panelLights.Length) SceneObjectsManager.instance.HideObjects();
        else
        {
            currentPuzzleIndex = index;
            for (int i = 0; i < panelLights.Length; i++)
            {
                if(i < currentPuzzleIndex)
                {
                    if(lightsOn[i] == false)
                    {
                        lightsOn[i] = true;
                        panelLights[i].GetComponent<Animator>().SetTrigger("fadeIn");
                    }
                }
                else if(lightsOn[i] == true)
                {
                    lightsOn[i] = false;
                    panelLights[i].GetComponent<Animator>().SetTrigger("fadeOut");
                }
            }
        }
    }
}
