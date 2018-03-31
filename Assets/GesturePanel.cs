using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GesturePanel : MonoBehaviour {

    public string myGestureName;
    public GameObject gestureSystem;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        gestureSystem.SetActive(true);
    }
}
