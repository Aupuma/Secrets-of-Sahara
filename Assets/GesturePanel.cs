using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GesturePanel : NetworkBehaviour {

    public string gestureName;
    private bool solved;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PanelTouched()
    {
        GestureManager.instance.ShowPanel(this);
    }

    [Command]
    public void CmdShowSolutionOverNetwork()
    {
        RpcShowSolutionLocally();
    }

    [ClientRpc]
    public void RpcShowSolutionLocally()
    {
        solved = true;
        GetComponent<MeshRenderer>().material.color = Color.green;
        //Comunico al gameManager que he sido resuelto para que lo comunique en red
    }
}
