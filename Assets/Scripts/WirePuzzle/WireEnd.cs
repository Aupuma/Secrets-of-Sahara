using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireEnd : WireObj {

    public MeshRenderer endMesh;
    public Material solutionConnexionMaterial;
    private Material originalMaterial;
    public bool connected = false;

    private void Start()
    {
        originalMaterial = endMesh.material;    
    }

    public void ConnectAndCheckIfSolved(Material mat)
    {
        if(mat == solutionConnexionMaterial)
        {
            connected = true;
            WirePuzzleManager.instance.CheckIfSolved();
            endMesh.material = mat;
        }
    }

    public void Disconnect()
    {
        connected = false;
        endMesh.material = originalMaterial;
    }
}
