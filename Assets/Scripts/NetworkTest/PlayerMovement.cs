using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using System;

public enum SwipeDirection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
}

public class PlayerMovement : NetworkBehaviour {

    bool canAct = true;
    public float actionTime = 0.5f;

    private SwipeDirection direction;
    private Vector3 touchPosition;
    private float swipeResistanceX = 50.0f;
    private float swipeResistanceY = 100.0f;

    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    float shakeDetectionThreshold = 2.0f;
    float lowPassFilterFactor;
    Vector3 lowPassValue;

    private void Start()
    {
        if (!hasAuthority)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        SetAccelerometer();
    }

    // Update is called once per frame
    void Update () {
        if (!hasAuthority)
        {
            return;
        }
        if(canAct) CheckShake();
        if(canAct) CheckSwipe();
        CheckIfTouchedPanel();
    }

    #region Input Handlers
    private void SetAccelerometer()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    private void CheckShake()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, fwd, out hit, 2f))
                return;
            else
                CmdDash();
            Debug.Log("Shake event detected at time " + Time.time);
        }
    }

    private void CheckSwipe()
    {
        direction = SwipeDirection.None;

        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 deltaSwipe = touchPosition - Input.mousePosition;

            if (Mathf.Abs(deltaSwipe.x) > swipeResistanceX)
            {
                //Swipe on the X axis
                direction |= (deltaSwipe.x < 0) ? SwipeDirection.Right : SwipeDirection.Left;
            }

            if (Mathf.Abs(deltaSwipe.y) > swipeResistanceY)
            {
                //Swipe on the Y axis
                direction |= (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }

        if (isSwiping(SwipeDirection.Up))
        {
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, fwd, out hit, 1f))
            {
                Debug.DrawRay(transform.position,fwd,Color.red,2f);
                print("There is something in front of the object!");
            }
            else
                CmdMove();
        }
        else if (isSwiping(SwipeDirection.Left))
        {
            CmdRotateLeft();
        }
        else if (isSwiping(SwipeDirection.Right))
        {
            CmdRotateRight();
        }
    }

    private bool isSwiping(SwipeDirection dir)
    {
        return (direction & dir) == dir;
    }

    private void CheckIfTouchedPanel()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, fwd, out hit, 1f))
            {
                if (hit.collider.tag == "GesturePanel")
                {
                    hit.collider.GetComponent<GesturePanel>().PanelTouched();
                }
            }
        }
    }

    #endregion

    #region Network Methods
    [Command]
    public void CmdRotateRight()
    {
        RpcRotateRight();
    }

    [ClientRpc]
    public void RpcRotateRight()
    {
        canAct = false;
        transform.DORotate(new Vector3(0f, transform.localEulerAngles.y + 90f, 0f), actionTime,
            RotateMode.Fast).OnComplete(ActionFinished);
    }

    [Command]
    public void CmdRotateLeft()
    {
        RpcRotateLeft();
    }

    [ClientRpc]
    public void RpcRotateLeft()
    {
        canAct = false;
        transform.DORotate(new Vector3(0f, transform.localEulerAngles.y - 90f, 0f), actionTime,
            RotateMode.Fast).OnComplete(ActionFinished);
    }

    [Command]
    public void CmdMove()
    {
        RpcMove();
    }

    [ClientRpc]
    public void RpcMove()
    {
        canAct = false;
        transform.DOMove(transform.position + transform.forward, actionTime).OnComplete(ActionFinished);
    }

    [Command]
    public void CmdDash()
    {
        RpcDash();
    }

    [ClientRpc]
    public void RpcDash()
    {
        canAct = false;
        transform.DOMove(transform.position + transform.forward * 2, actionTime).OnComplete(ActionFinished);
    }

    void ActionFinished()
    {
        canAct = true;
    } 
    #endregion
}
