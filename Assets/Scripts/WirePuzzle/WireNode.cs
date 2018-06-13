using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireNode : MonoBehaviour {

    public WireNode internalConnection; //Nodo del lado opuesto del cable
    [HideInInspector] public WireNode externalConnection; //Nodo que se conectará externamente
    [HideInInspector] public MeshRenderer wireRenderer;
    [HideInInspector] public Material originalMaterial;
    [HideInInspector] public Material currentConnexionMaterial;
     public bool connected = false;
    [HideInInspector] public int connectionOrder = -1;

    // Use this for initialization
    public virtual void Start () {
        wireRenderer = transform.parent.GetComponent<MeshRenderer>();
        originalMaterial = wireRenderer.material;
	}

    public virtual void Connect(WireNode callNode,int callOrder, Material conexMaterial)
    {
        connected = true;
        currentConnexionMaterial = conexMaterial;
        connectionOrder = callOrder;

        if (callNode == internalConnection) 
        {
            //Si el nodo que dice que nos conectemos es el del otro lado del cable
            //le decimos al nodo externo (si hay) que se conecte
            if (externalConnection != null) externalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
        else 
        {
            //Si el nodo que ha llamado es el nodo externo, asignamos material de conexión y
            //le decimos al nodo del otro lado de nuestro cable que se conecte
            wireRenderer.material = conexMaterial;
            internalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WireNode")
        {
            externalConnection = other.GetComponent<WireNode>();

            if (connected && externalConnection.connected==false)
                externalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
    }

    public virtual void Disconnect(WireNode callNode)
    {
        connected = false;
        connectionOrder = -1;

        if (callNode == internalConnection) //Si la desconexión se llama desde el nodo interno
        {
            if (externalConnection != null) externalConnection.Disconnect(this);
        }
        else //Si la desconexión se ha pedido desde el nodo externo
        {
            wireRenderer.material = originalMaterial;
            internalConnection.Disconnect(this);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.tag == "WireNode")
        {
            //Si estamos conectados y hay nodo externo conectado a nosotros 
            //si su orden de conexion es posterior le mandamos a desconectar
            if (connected && externalConnection.connectionOrder > connectionOrder)
                externalConnection.Disconnect(this);
            externalConnection = null;
        }
    }
}
