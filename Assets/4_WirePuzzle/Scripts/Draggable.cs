using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TransformationType
{
    rotation,
    movementZ,
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
        else if (transfChoice == TransformationType.movementZ)
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
        if (selected)
        {
            if (transfChoice == TransformationType.rotation) CheckRotation();
            else if (transfChoice == TransformationType.movementZ) CheckMovementZ();
            else if (transfChoice == TransformationType.movementY) CheckMovementY();
        }
        else if(autoSnaps) //Si no está seleccionado, movemos a lo más próximo
        {
            if (transfChoice == TransformationType.rotation && !onRotationInterval) RotateToNearest();
            else if (transfChoice == TransformationType.movementZ && !onPositionInterval) MoveToNearestZ();
            else if (transfChoice == TransformationType.movementY && !onPositionInterval) MoveToNearestY();
        }
    }

    #region PLAYER INPUT

    // PARA UN MOVIMIENTO CORRECTO CADA DRAGGABLE DEBERÍA TENER GUARDADA LA POSICION DE SUS LIMITES
    // HABRÍA QUE PROYECTAR LA POSICION EN EL CANVAS Y CALCULAR SU DISTANCIA, SI ES MAYOR EN X, EL MOVIMIENTO SE COGERÁ EN X,
    // SI NO, EL MOVIMIENTO SE COGERÁ EN Y
    //PARA MOVER EL OBJETO HABRÁ QUE SABER HACIA CUAL PUNTO DE LOS LIMITES ESTAMOS MOVIENDO EL RATON

    private void CheckMovementY()
    {
        //Reiniciamos variables
        nearestIntervalPosObtained = false;
        onPositionInterval = false;

        var localUp = objTrans.InverseTransformDirection(objTrans.up);

        //Obtenemos input del player
        float movY = Input.GetAxis("Mouse Y") * userMovSpeed;
        if (detectsCollisions) rb.velocity = localUp * movY;
        else objTrans.Translate(0, movY * Time.deltaTime, 0, Space.Self);

        //Corregimos la posicion si nos pasamos del límite
        CorrectPositionY();
    }

    private void CheckMovementZ()
    {
        nearestIntervalPosObtained = false;
        onPositionInterval = false;

        var localDir = objTrans.InverseTransformDirection(objTrans.forward);

        float movZ = Input.GetAxis("Mouse X") * userMovSpeed;
        if (detectsCollisions) rb.velocity = localDir * movZ * -1;
        else objTrans.Translate(0, 0, movZ * Time.deltaTime *-1, Space.Self);

        CorrectPositionZ();
    }

    private void CheckRotation()
    {
        onRotationInterval = false;

        float rotY = -Input.GetAxis("Mouse X") * userRotSpeed;
        objTrans.Rotate(Vector3.up, rotY);

        CorrectRotation();
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

    private void CorrectPositionZ()
    {
        if (!selected && Mathf.Abs(objTrans.localPosition.z - nearestIntervalPos) < 0.05f)
        {
            objTrans.localPosition = new Vector3(objTrans.localPosition.x, objTrans.localPosition.y, nearestIntervalPos);
            onPositionInterval = true;
        }

        if (objTrans.localPosition.z > maxPosition)
        {
            objTrans.localPosition = new Vector3(objTrans.localPosition.x, objTrans.localPosition.y, maxPosition);
            onPositionInterval = true;
        }
        else if (objTrans.localPosition.z < minPosition)
        {
            objTrans.localPosition = new Vector3(objTrans.localPosition.x, objTrans.localPosition.y, minPosition);
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

    private void MoveToNearestZ()
    {
        if (!nearestIntervalPosObtained) GetNearestIntervalPoint(objTrans.localPosition.z);

        Vector3 lastPos = objTrans.localPosition;

        if (objTrans.localPosition.z < nearestIntervalPos) objTrans.Translate(0, 0, snapMovSpeed * Time.deltaTime, Space.Self);
        else if (objTrans.localPosition.z > nearestIntervalPos) objTrans.Translate(0, 0, -snapMovSpeed * Time.deltaTime, Space.Self);

        CorrectPositionZ();
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
            objTrans.Rotate(Vector3.up, snapRotSpeed);
        }
        else
        {
            objTrans.Rotate(Vector3.up, -snapRotSpeed);
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
