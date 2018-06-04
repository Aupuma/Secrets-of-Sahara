using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RotatingPuzzleManager : NetworkBehaviour {

    public static RotatingPuzzleManager instance;

    public float rotationTime = 0.35f;
    public Transform[] puzzlePieces;
    public Transform[] pillars;

    [SyncVar]
    private bool isRotating = false;

    // Use this for initialization
    void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {  
        if (!isServer && !isRotating && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            // Detectamos el touch en las piezas
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "RotatingPiece")
                {
                    int rotIndex = int.Parse(hit.collider.name.Substring(hit.collider.name.Length - 1));
                    Debug.Log("RotIndex: " + rotIndex);
                    CmdRotateElements(rotIndex);
                }
            }
        }
    }

    [Command]
    public void CmdRotateElements(int index)
    {
        RpcRotateElements(index);
    }

    [ClientRpc]
    public void RpcRotateElements(int index)
    {
        if (isServer) //Rotamos el pilar en el jugador AR
        {
            Debug.Log("rotating pillar");
            pillars[index].DOLocalRotate(new Vector3(
                pillars[index].localEulerAngles.x,
                pillars[index].localEulerAngles.y + 90f,
                pillars[index].localEulerAngles.z), 
                rotationTime).
                OnComplete(CmdRotationFinished); 
        }
        else //Rotamos la pieza en el jugador POV
        {
            Debug.Log("rotating piece");
            puzzlePieces[index].DOLocalRotate(new Vector3(
                puzzlePieces[index].localEulerAngles.x,
                puzzlePieces[index].localEulerAngles.y,
                puzzlePieces[index].localEulerAngles.z + 90f),
                rotationTime);
        }
    }

    [Command]
    public void CmdRotationFinished()
    {
        isRotating = false;
        foreach (var pillar in pillars)
        {
            if (pillar.localEulerAngles.y != 0) return;
        }
        SceneObjectsManager.instance.HideObjects();
    }
}
