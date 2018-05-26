using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePuzzleManager : MonoBehaviour {

    public static WirePuzzleManager instance;
    public WireEnd[] wireEnds;

	// Use this for initialization

	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CheckIfSolved()
    {
        foreach (var piece in wireEnds)
        {
            if (!piece.connected) return; //Si hay alguna incorrecta finalizamos
        }
        SceneObjectsManager.instance.HideObjects();
        //Si llegamos al final del bucle es que todas son correctas

    }
}
