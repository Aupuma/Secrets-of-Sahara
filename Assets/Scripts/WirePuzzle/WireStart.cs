using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireStart : WireNode {

    public Material connectionMaterial;
    private WireNode currentConnectedNode;

    public override void Start()
    {
        base.Start();
        connected = true;
        connectionOrder = 0;
        currentConnexionMaterial = connectionMaterial;
    }
}
