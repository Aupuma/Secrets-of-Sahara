using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireStart : WireObj {

    public Material connectionMaterial;
    private WireNode currentConnectedNode;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WireNode")
        {
            currentConnectedNode = other.GetComponent<WireNode>();
            currentConnectedNode.Connect(this,0, connectionMaterial);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "WireNode")
        {
            currentConnectedNode.Disconnect(this);
            currentConnectedNode = null;
        }
    }
}
