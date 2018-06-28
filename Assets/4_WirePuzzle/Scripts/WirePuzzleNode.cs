using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePuzzleNode : WireNode {

    public int orderInPuzzle;
    public Animator textureAnimator;

    public override void Connect(WireNode callNode, int callOrder, Material conexMaterial)
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
        else if (orderInPuzzle == WirePuzzleManager.instance.currentPuzzleIndex)
        {
            WirePuzzleManager.instance.NextNodeConnected();
            //Si el nodo que ha llamado es el nodo externo, asignamos material de conexión y
            //le decimos al nodo del otro lado de nuestro cable que se conecte
            wireRenderer.material = conexMaterial;
            textureAnimator.SetTrigger("fadeIn");
            internalConnection.Connect(this, connectionOrder + 1, currentConnexionMaterial);
        }
    }

    public override void Disconnect(WireNode callNode)
    {
        connected = false;
        connectionOrder = -1;

        if (callNode == internalConnection) //Si la desconexión se llama desde el nodo interno
        {
            if (externalConnection != null) externalConnection.Disconnect(this);
        }
        else //Si la desconexión se ha pedido desde el nodo externo
        {
            WirePuzzleManager.instance.NodeLostConnexion();
            wireRenderer.material = originalMaterial;
            textureAnimator.SetTrigger("fadeOut");
            internalConnection.Disconnect(this);
        }
    }
}
