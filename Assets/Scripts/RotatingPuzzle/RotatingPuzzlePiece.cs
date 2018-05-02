using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingPuzzlePiece : MonoBehaviour {

    public bool isCorrect = false;
    public float correctRotation;
    public static float rotationTime = 0.35f;
    private bool isRotating = false;

    private void Start()
    {
        if (transform.localEulerAngles.z == correctRotation) isCorrect = true;
    }

    public void Rotate()
    {
        if (!isRotating)
        {
            isRotating = true;
            transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 90f),
                rotationTime).
                OnComplete(RotationFinished);
        }
    }
    
    public void RotationFinished()
    {
        if (transform.localEulerAngles.z == correctRotation)
        {
            isCorrect = true;
            RotatingPuzzleManager.instance.CheckIfSolved();
        }
        else isCorrect = false;
        isRotating = false;
    }
}
