using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireEnd : WireObj {

    public MeshRenderer endMesh;
    public Material solutionConnexionMaterial;
    private Material originalMaterial;
    public bool solved = false;
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
            endMesh.material = mat;

            if (!solved)
            {
                solved = true;
                //LLAMAMOS A EVENTO DE RESOLUCIÓN
            }
        }
    }

    public void Disconnect()
    {
        connected = false;
        endMesh.material = originalMaterial;
    }
}
