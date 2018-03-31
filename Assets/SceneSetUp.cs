using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Networking;

public class SceneSetUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.transform.DOMoveY(-2, 2f).From().OnComplete(SceneReady);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SceneReady()
    {
        NetDiscovery.instance.StartAsServer();
        NetworkManager.singleton.StartHost();
    }
}
