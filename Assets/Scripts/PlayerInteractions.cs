using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractions : NetworkBehaviour{

    private Transform raycastInitialPos;
    public Transform drawingTrailPos;

	// Use this for initialization
	void Start () {
        raycastInitialPos = transform.GetChild(1);

        if (isServer)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!hasAuthority)
        {
            return;
        }
        CheckIfTouchedPanel();
    }

    private void CheckIfTouchedPanel()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 fwd = raycastInitialPos.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(raycastInitialPos.position, fwd, out hit, 1f))
            {
                if (hit.collider.tag == "GesturePanel")
                {
                    //hit.collider.GetComponent<GesturePanel>().PanelTouched();
                }
            }
        }
    }
}
