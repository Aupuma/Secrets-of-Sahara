using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SceneObjectsManager : NetworkBehaviour {

    public GameObject[] AR_Player_Objects;
    public GameObject[] POV_Player_Objects;

	// Use this for initialization
	void Start () {
        if (isServer) //Somos el jugador en AR
        {
            foreach (var obj in POV_Player_Objects)
            {
                obj.SetActive(false);
            }
        }
        else //Somos el jugador en primera persona
        {
            foreach (var obj in AR_Player_Objects)
            {
                obj.SetActive(false);
            }
        }
	}
}
