using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GesturePanel : MonoBehaviour {

    public string gestureName;
    GestureManager gestureManager;
    private bool solved;

	// Use this for initialization
	void Start () {
        gestureManager = FindObjectOfType<GestureManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        gestureManager.ShowPanel(this);
    }

    public void SolutionFound()
    {
        solved = true;
        GetComponent<MeshRenderer>().material.color = Color.green;
        //Comunico al gameManager que he sido resuelto para que lo comunique en red
    }
}
