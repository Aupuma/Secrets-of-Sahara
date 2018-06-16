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

public class POVPlayerInteractions : NetworkBehaviour {

    #region DATA

    public PlayerConnectionObject connection;
    private Vector3 respawnPos;
    private Quaternion respawnRot;

    [Header("Parameters")]
    bool canAct = true;
    public float actionTime = 0.5f;
    public float movementDistance = 1.5f;

    private SwipeDirection direction;
    private Vector3 touchPosition;
    private float swipeResistanceX = 50.0f;
    private float swipeResistanceY = 100.0f;

    private Transform raycastInitialPos; 
    #endregion

    #region SINGLETON
    public static POVPlayerInteractions instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region MONOBEHAVIOUR METHODS
    private void Start()
    {
        if (isServer)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            respawnPos = transform.position;
            respawnRot = transform.rotation;
        }
        else
        {
            raycastInitialPos = transform.GetChild(1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) //Si no tiene autoridad (servidor) no hacemos nada
        {
            return;
        }
        else if (canAct) CheckSwipe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.tag == "Key")
            {
                NetworkServer.Destroy(other.gameObject);
                MazeManager.instance.RpcUnlockElements();

                //Nueva posición de spawneo
                respawnPos = transform.position;
                respawnRot = transform.rotation;
            }
            else if (other.gameObject.tag == "Door")
            {
                MazeManager.instance.RpcMazeCompleted();
            }
            else if (other.gameObject.tag == "Trap")
            {
                RpcActivateTrapAndRestart(other.gameObject);
            }
        }
    }

    private void Restart()
    {
        transform.position = respawnPos;
        transform.rotation = respawnRot;
    }


    #endregion

    #region INPUT HANDLERS

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
            Vector3 fwd = raycastInitialPos.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(raycastInitialPos.position, fwd, out hit, 1f))
            {
                Debug.DrawRay(raycastInitialPos.position,fwd,Color.red,2f);
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
    #endregion

    #region NETWORK METHODS

    [Command]
    public void CmdRotateRight()
    {
        RpcRotateRight();
    }

    [ClientRpc]
    public void RpcRotateRight()
    {
        canAct = false;
        transform.DORotate(new Vector3(0f, transform.localEulerAngles.y + 90f, 0f), 
            actionTime,
            RotateMode.Fast).
            OnComplete(ActionFinished);
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
        transform.DORotate(new Vector3(0f, transform.localEulerAngles.y - 90f, 0f), 
            actionTime,
            RotateMode.Fast).
            OnComplete(ActionFinished);
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
        transform.DOMove(transform.position + transform.forward * movementDistance, 
            actionTime).
            OnComplete(ActionFinished);
    }

    [ClientRpc]
    public void RpcActivateTrapAndRestart(GameObject trap)
    {
        trap.GetComponent<Animator>().SetTrigger("Move");
        Invoke("Restart", 1f);
    }

    void ActionFinished()
    {
        canAct = true;
    }
    #endregion

}
