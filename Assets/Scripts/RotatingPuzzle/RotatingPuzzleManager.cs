using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RotatingPuzzleManager : NetworkBehaviour {

    public static RotatingPuzzleManager instance;
    public RotatingPuzzlePiece[] puzzlePieces;
    public GameObject[] pillars;
    public bool solved = false;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {  
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "RotatingPiece")
                {
                    hit.collider.GetComponent<RotatingPuzzlePiece>().Rotate();
                }
            }
        }
    }

    public void CheckIfSolved()
    {
        foreach (var piece in puzzlePieces)
        {
            if (!piece.isCorrect) return; //Si hay alguna incorrecta finalizamos
        }
        SceneObjectsManager.instance.HideObjects();
        //Si llegamos al final del bucle es que todas son correctas

    }
}
