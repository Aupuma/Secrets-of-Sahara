using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TransformationType
{
    rotation,
    movementX,
    movementY
}

public class Draggable : MonoBehaviour {

    [Header("General parameters")]//---------------------------------------------
    public TransformationType transfChoice;
    public bool autoSnaps = true;
    public bool detectsCollisions = false;

    [Header("Rotation parameters")]//---------------------------------------------
    public float userRotSpeed = 2.5f;
    public float snapRotSpeed = 2.5f;
    private bool onRotationInterval = true;

    [Header("Movement parameters")]//---------------------------------------------
    public float userMovSpeed = 2.5f;
    public float snapMovSpeed = 2.5f;
    public float minPosition;
    public float maxPosition;
    public float moveIntervalDistance = 1;
    private float nearestIntervalPos;
    private bool nearestIntervalPosObtained;
    private bool onPositionInterval;

    private Transform objTrans;
    private Rigidbody rb;
    private Collider objCollider;

    private bool selected = false;

    [HideInInspector]
    public bool Selected
    {
        get
        {
            return selected;
        }

        set
        {
            selected = value;
            if (value == true) rb.isKinematic = false;
            else rb.isKinematic = true;
        }
    }

    // Use this for initialization
    void Start () {
        objTrans = transform;
        rb = GetComponent<Rigidbody>();
        objCollider = GetComponent<Collider>();
        SetRigidbodyConstraints();
    }

    void SetRigidbodyConstraints()
    {
        if (transfChoice == TransformationType.rotation)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }
        else if (transfChoice == TransformationType.movementX)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }
        else if (transfChoice == TransformationType.movementY)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX
                | RigidbodyConstraints.FreezePositionZ;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(selected);
        if (selected)
        {
            if (transfChoice == TransformationType.rotation) CheckRotation();
            else if (transfChoice == TransformationType.movementX) CheckMovementX();
            else if (transfChoice == TransformationType.movementY) CheckMovementY();
        }
        else if(autoSnaps) //Si no está seleccionado, movemos a lo más próximo
        {
            if (transfChoice == TransformationType.rotation && !onRotationInterval) RotateToNearest();
            else if (transfChoice == TransformationType.movementX && !onPositionInterval) MoveToNearestX();
            else if (transfChoice == TransformationType.movementY && !onPositionInterval) MoveToNearestY();
        }
    }

    #region PLAYER INPUT
    private void CheckMovementY()
    {
        //Reiniciamos variables
        nearestIntervalPosObtained = false;
        onPositionInterval = false;

        //Obtenemos input del player
        float movY = Input.GetAxis("Mouse Y") * userMovSpeed;
        if (detectsCollisions) rb.velocity = objTrans.up * movY;
        else objTrans.Translate(0, movY * Time.deltaTime, 0);

        //Corregimos la posicion si nos pasamos del límite
        CorrectPositionY();
    }

    private void CheckMovementX()
    {
        nearestIntervalPosObtained = false;
        onPositionInterval = false;

        Vector3 lastPos = objTrans.localPosition;
        var localRight = objTrans.InverseTransformDirection(objTrans.right);

        float movX = Input.GetAxis("Mouse X") * userMovSpeed;
        if (detectsCollisions) rb.velocity = localRight * movX;
        else objTrans.Translate(movX * Time.deltaTime, 0, 0,Space.Self);

        CorrectPositionX();
    }

    private void CheckRotation()
    {
        onRotationInterval = false;

        float rotY = -Input.GetAxis("Mouse X") * userRotSpeed * Time.deltaTime;
        objTrans.Rotate(Vector3.up, rotY);

        CorrectRotation();

        /*
        if (-Input.GetAxis("Mouse X") > 0 && onRightRotationLimit) return;
        if (-Input.GetAxis("Mouse X") < 0 && onLeftRotationLimit) return;

        if(-Input.GetAxis("Mouse X") > 0)
        {
            onLeftRotationLimit = false;
        }
        if (-Input.GetAxis("Mouse X") < 0)
        {
            onRightRotationLimit = false;
        }
        */
    }
    #endregion //PLAYER INPUT

    #region MOVEMENT CORRECTION
    private void CorrectPositionY()
    {
        //Si estamos cerca del punto de intervalo ajustamos la posición a ese punto
        if (!selected && Mathf.Abs(objTrans.localPosition.y - nearestIntervalPos) < 0.05f)
        {
            objTrans.localPosition = new Vector3(objTrans.localPosition.x, nearestIntervalPos, objTrans.localPosition.z);
            onPositionInterval = true;
        }

        //Si nos hemos pasado del máximo o mínimo ajustamos
        if (objTrans.localPosition.y > maxPosition)
        {
            objTrans.localPosition = new Vector3(objTrans.localPosition.x, maxPosition, objTrans.localPosition.z);
            onPositionInterval = true;
        }
        else if (objTrans.localPosition.y < minPosition)
        {
            objTrans.localPosition = new Vector3(objTrans.localPosition.x, minPosition, objTrans.localPosition.z);
            onPositionInterval = true;
        }
    }

    private void CorrectPositionX()
    {
        if (!selected && Mathf.Abs(objTrans.localPosition.x - nearestIntervalPos) < 0.05f)
        {
            objTrans.localPosition = new Vector3(nearestIntervalPos, objTrans.localPosition.y, objTrans.localPosition.z);
            onPositionInterval = true;
        }

        if (objTrans.localPosition.x > maxPosition)
        {
            objTrans.localPosition = new Vector3(maxPosition, objTrans.localPosition.y, objTrans.localPosition.z);
            onPositionInterval = true;
        }
        else if (objTrans.localPosition.x < minPosition)
        {
            objTrans.localPosition = new Vector3(minPosition, objTrans.localPosition.y, objTrans.localPosition.z);
            onPositionInterval = true;
        }
    }

    private void CorrectRotation()
    {
        //Si la rotación está en el límite la ajustamos
        float rotY = objTrans.localEulerAngles.y;
        float nearestAngle = Mathf.Round(rotY / 90) * 90;

        if (Mathf.Abs(nearestAngle - rotY) < 4)
        {
            objTrans.localRotation = Quaternion.Euler(0, nearestAngle, 0);
            onRotationInterval = true;
        }
        

        /*
        if (rotY > 180f && 360 - rotY > leftRotation)
        {
            rotY = 360-leftRotation;
            onLeftRotationLimit = true;
        }
        else if (rotY < 180f && rotY > rightRotation)
        {
            rotY = rightRotation;
            onRightRotationLimit = true;
        }

        objTrans.localRotation = Quaternion.Euler(0, rotY, 0);
        */
    }
    #endregion //MOVEMENT CORRECTION

    #region AUTOMATIC SNAPPING
    private void MoveToNearestY()
    {
        if (!nearestIntervalPosObtained) GetNearestIntervalPoint(objTrans.localPosition.y);

        Vector3 lastPos = objTrans.localPosition;

        if (objTrans.localPosition.y < nearestIntervalPos) objTrans.Translate(0, snapMovSpeed * Time.deltaTime, 0);
        else if (objTrans.localPosition.y > nearestIntervalPos) objTrans.Translate(0, -snapMovSpeed * Time.deltaTime, 0);

        CorrectPositionY();
    }

    private void MoveToNearestX()
    {
        if (!nearestIntervalPosObtained) GetNearestIntervalPoint(objTrans.localPosition.x);

        Vector3 lastPos = objTrans.localPosition;

        if (objTrans.localPosition.x < nearestIntervalPos) objTrans.Translate(snapMovSpeed * Time.deltaTime, 0, 0, Space.Self);
        else if (objTrans.localPosition.x > nearestIntervalPos) objTrans.Translate(-snapMovSpeed * Time.deltaTime, 0, 0, Space.Self);

        CorrectPositionX();
    }

    private void GetNearestIntervalPoint(float currentPos)
    {
        float intervalPos = minPosition;
        float minDist = float.MaxValue;
        while (intervalPos <= maxPosition)
        {
            float currentDist = Mathf.Abs(currentPos - intervalPos);
            if (currentDist < minDist)
            {
                minDist = currentDist;
                nearestIntervalPos = intervalPos;
            }
            intervalPos += moveIntervalDistance;
        }
        nearestIntervalPosObtained = true;
    }

    private void RotateToNearest()
    {
        float rotY = objTrans.localEulerAngles.y;
        float nearestAngle = Mathf.Round(rotY / 90) * 90;

        if (nearestAngle > rotY)
        {
            objTrans.Rotate(Vector3.up, snapRotSpeed * Time.deltaTime);
        }
        else
        {
            objTrans.Rotate(Vector3.up, -snapRotSpeed * Time.deltaTime);
        }

        CorrectRotation();
        /*
        float angleToLeftLimit = 0;
        float angleToRightLimit = 0;

        if (rotY > 180) //Esta en el lado izquierdo
        {
            rotY = 360 - rotY;

            angleToRightLimit = rightRotation + rotY;
            angleToLeftLimit = Math.Abs(rotY - leftRotation);
        }
        else //Está en el lado derecho
        {
            angleToLeftLimit = leftRotation + rotY;
            angleToRightLimit = Math.Abs(rotY - rightRotation);
        }

        if(angleToLeftLimit < angleToRightLimit)
        {
            objTrans.Rotate(Vector3.up, -snapRotSpeed * Time.deltaTime);
        }
        else
        {
            objTrans.Rotate(Vector3.up, snapRotSpeed * Time.deltaTime);
        }

        CorrectRotation();
        */
    }
    #endregion //AUTOMATIC SNAPPING
}
