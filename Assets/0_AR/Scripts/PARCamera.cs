using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PARCamera : MonoBehaviour {

    private static PARCamera _instance;
    public static PARCamera Instance { get { return _instance; } }

    private GameObject drawingPlane;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        drawingPlane = transform.GetChild(0).gameObject;
        DontDestroyOnLoad(this.gameObject);
    }

    public void EnableDrawingPlane()
    {
        drawingPlane.SetActive(true);
    }
}
