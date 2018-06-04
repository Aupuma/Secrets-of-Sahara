using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractions : NetworkBehaviour {

    private Transform raycastInitialPos;
    public Transform drawingTrailPos;
    public PlayerConnectionObject myConnection;

    public static PlayerInteractions instance;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);

        raycastInitialPos = transform.GetChild(1);

        if (isServer)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }

        instance = this;
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
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 fwd = raycastInitialPos.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(raycastInitialPos.position, fwd, out hit, 1f))
            {
                if (hit.collider.tag == "TrapButton")
                {
                    myConnection.CmdActivateRemoteTraps();
                }
            }
        }
        */
        /*
        RaycastHit hit = new RaycastHit();
        if (Input.GetMouseButtonDown(0))
        {
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "TrapButton")
                {
                    TrapButton.instance.ButtonPressed();
                    myConnection.CmdActivateRemoteTraps();
                }
            }
        }*/
    }
    
    [Command]
    public void CmdRotationCall(int index)
    {
        RotatingPuzzleManager.instance.RpcRotateElements(index);
    }
}
