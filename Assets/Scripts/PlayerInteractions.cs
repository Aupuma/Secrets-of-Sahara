using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractions : NetworkBehaviour{

	// Use this for initialization
	void Start () {
        if (!hasAuthority)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        else
        {
            FindObjectOfType<GestureManager>().SetCamera(GetComponentInChildren<Camera>());
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
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, fwd, out hit, 1f))
            {
                if (hit.collider.tag == "GesturePanel")
                {
                    hit.collider.GetComponent<GesturePanel>().PanelTouched();
                }
            }
        }
    }
}
