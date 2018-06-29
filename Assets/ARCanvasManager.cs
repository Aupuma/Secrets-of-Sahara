using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCanvasManager : MonoBehaviour {

    public GameObject movePhoneIcon;
    public PlaceOnPlane objPlacer;
    public ARPlaneManager planeManager;

    float lastCheckTime;
    float secsInterval;

	// Use this for initialization
	void Start () {
        lastCheckTime = Time.time;
        secsInterval = 2f;
	}
	
	// Update is called once per frame
	void Update () {

        if (movePhoneIcon.activeSelf)
        {
            if (Time.time > lastCheckTime + secsInterval)
            {
                if (planeManager.planeCount > 0)
                {
                    movePhoneIcon.SetActive(false);
                    objPlacer.enabled = true;
                }
                else lastCheckTime = Time.time;
            }
        }

	}
}
