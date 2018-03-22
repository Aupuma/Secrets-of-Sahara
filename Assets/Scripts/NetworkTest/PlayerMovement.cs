using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class PlayerMovement : NetworkBehaviour {

    bool canAct = true;
    public float actionTime = 0.5f;

	// Update is called once per frame
	void Update () {
        if (hasAuthority && canAct)
        {
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
            }
        }
    }

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
}
