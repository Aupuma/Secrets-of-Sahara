using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireNode : MonoBehaviour {

    public WireNode internalConnection; //Nodo del lado opuesto del cable
    [HideInInspector] public WireNode externalConnection; //Nodo que se conectará externamente
    [HideInInspector] public MeshRenderer wireRenderer;
    [HideInInspector] public Material originalMaterial;
    [HideInInspector] public Material currentConnexionMaterial;
    [HideInInspector] public bool connected = false;
    [HideInInspector] public int connectionOrder = -1;

    public virtual void Start () {
        wireRenderer = transform.parent.GetComponent<MeshRenderer>();
        originalMaterial = wireRenderer.material;
	}

    /// <summary>
    /// Conectamos este nodo, y depende del nodo que nos haya llamado, 
    /// continuamos con la llamada recursiva de conexión en una dirección u otra
    /// </summary>
    /// <param name="callNode"></param>
    /// <param name="callOrder"></param>
    /// <param name="conexMaterial"></param>
    public virtual void Connect(WireNode callNode,int callOrder, Material conexMaterial)
    {
        connected = true;
        currentConnexionMaterial = conexMaterial;
        connectionOrder = callOrder;

        if (callNode == internalConnection) 
        {
            if (externalConnection != null) externalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
        else 
        {
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

    /// <summary>
    /// Desconectamos este nodo y si tenemos alguno conectado a nosotros, seguimos desconectando de manera recursiva
    /// </summary>
    /// <param name="callNode"></param>
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
