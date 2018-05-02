using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireNode : WireObj {

    public WireNode internalConnection; //Nodo del lado opuesto del cable

    [HideInInspector]
    public WireNode externalConnection; //Nodo que se conectará externamente

    private WireEnd currentWireEnd; 

    public bool connected = false;

    private MeshRenderer wireRenderer;
    private Material originalMaterial;
    private Material currentConnexionMaterial;

    public int connectionOrder;

	// Use this for initialization
	void Start () {
        wireRenderer = transform.parent.GetComponent<MeshRenderer>();
        originalMaterial = wireRenderer.material;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Connect(WireObj callNode,int callOrder, Material conexMaterial)
    {
        connected = true;
        currentConnexionMaterial = conexMaterial;
        connectionOrder = callOrder;

        if (callNode == internalConnection) 
        {
            //Si el nodo que dice que nos conectemos es el del otro lado del cable
            //le decimos al nodo externo (si hay) que se conecte
            if (externalConnection != null) externalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
            else if (currentWireEnd != null) currentWireEnd.ConnectAndCheckIfSolved(currentConnexionMaterial);
        }
        else 
        {
            //Si el nodo que ha llamado es el nodo externo, asignamos material de conexión y
            //le decimos al nodo del otro lado de nuestro cable que se conecte
            wireRenderer.material = conexMaterial;
            internalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WireNode")
        {
            externalConnection = other.GetComponent<WireNode>();
            if (connected && externalConnection.connected==false) externalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
        else if(other.tag == "WireEnd")
        {
            currentWireEnd = other.GetComponent<WireEnd>();
            if (connected) currentWireEnd.ConnectAndCheckIfSolved(currentConnexionMaterial);
        }
    }

    public void Disconnect(WireObj callNode)
    {
        connected = false;
        if(callNode == internalConnection)
        {
            if (externalConnection != null) externalConnection.Disconnect(this);
            else if (currentWireEnd != null) currentWireEnd.Disconnect();
        }
        else
        {
            wireRenderer.material = originalMaterial;
            internalConnection.Disconnect(this);
        }
        connectionOrder = -1;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "WireNode")
        {
            if (connected && externalConnection.connectionOrder>connectionOrder)
                externalConnection.Disconnect(this);
            externalConnection = null;
        }
        else if(other.tag == "WireEnd")
        {
            if (connected) currentWireEnd.Disconnect();
        }
    }
}
