using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

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

    // Update is called once per frame
    void Update () {
        if (hasAuthority && canAct)
        {
            /*
            if (Input.GetAxis("Horizontal") == -1)
            {
                CmdRotateLeft();
            }
            if (Input.GetAxis("Horizontal") == 1)
            {
                CmdRotateRight();
            }
            if (Input.GetAxis("Vertical") == 1)
            {
                CmdMove();
            }*/

            if (isSwiping(SwipeDirection.Up))
            {
                CmdMove();
            }
            if (isSwiping(SwipeDirection.Left))
            {
                CmdRotateLeft();
            }
            if (isSwiping(SwipeDirection.Right))
            {
                CmdRotateRight();
            }
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
    }

    private bool isSwiping(SwipeDirection dir)
    {
        return (direction & dir) == dir;
    }

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

    void ActionFinished()
    {
        canAct = true;
    } 
    #endregion
}
