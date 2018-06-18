using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PARCamera : MonoBehaviour {

    public static PARCamera instance;
    private GameObject drawingPlane;

    private void Awake()
    {
        instance = this;
        drawingPlane = transform.GetChild(0).gameObject;
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this.gameObject);
	}

    public void EnableDrawingPlane()
    {
        drawingPlane.SetActive(true);
    }
}
