using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Networking;

public class SceneSetUp : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if (isServer)
        {
            this.transform.DOMoveY(-2, 2f).From().OnComplete(SceneReady);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SceneReady()
    {
        NetDiscovery.instance.StartAsServer();
    }
}
