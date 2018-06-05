using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireEnd : WireNode {

    public Material solutionConnexionMaterial;

    public override void Start()
    {
        base.Start();
    }

    public override void Connect(WireNode callNode, int callOrder, Material conexMaterial)
    {
        if (conexMaterial == solutionConnexionMaterial) {
            wireRenderer.material = conexMaterial;
            connected = true;
            connectionOrder = callOrder;
            //DESHABILITARIAMOS TODOS LOS HANDLES PARA QUE YA NO SE PUEDA MOVER
        }
    }

    public override void Disconnect(WireNode callNode)
    {
        connected = false;
        connectionOrder = -1;
        wireRenderer.material = originalMaterial;
    }
}
