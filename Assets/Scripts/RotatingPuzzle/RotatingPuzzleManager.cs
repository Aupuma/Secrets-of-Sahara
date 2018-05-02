using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPuzzleManager : MonoBehaviour {

    public static RotatingPuzzleManager instance;
    public RotatingPuzzlePiece[] puzzlePieces;
    public bool solved = false;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit = new RaycastHit();
        if (Input.GetMouseButtonDown(0))
        {
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
        solved = true;
        //Si llegamos al final del bucle es que todas son correctas

    }
}
