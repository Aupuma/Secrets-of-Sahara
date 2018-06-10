using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MazeManager : NetworkBehaviour {

    public static MazeManager instance;

    public Animator[] lockedElements;
    public Animator[] hiddenTrapsBeforeKey;
    public Animator[] hiddenTrapsAfterKey;

    private void Awake()
    {
        instance = this;
    }

    public void Ready()
    {
        if (isServer)
        {
            NetDiscovery.instance.StartAsServer();
        }
    }

    [Command]
    public void CmdEnableFirstTraps()
    {
        RpcEnableFirstTraps();
    }

    [ClientRpc]
    public void RpcEnableFirstTraps()
    {
        foreach (var item in hiddenTrapsBeforeKey)
        {
            item.SetTrigger("fadeIn");
        }
    }

    [Command]
    public void CmdUnlockElements()
    {
        RpcUnlockElements();
    }

    [ClientRpc]
    public void RpcUnlockElements()
    {
        foreach (var item in lockedElements)
        {
            Debug.Log("trying to meve");
            item.SetTrigger("Unlock"); //Movemos hacia abajo las paredes bloqueadas
        }
        foreach (var item in hiddenTrapsBeforeKey)
        {
            item.SetTrigger("fadeOut");
        }
        foreach (var item in hiddenTrapsAfterKey)
        {
            item.SetTrigger("fadeIn");
        }
    }

    [Command]
    public void CmdMazeCompleted()
    {
        RpcMazeCompleted();
    }

    [ClientRpc]
    public void RpcMazeCompleted()
    {
        foreach (var item in hiddenTrapsAfterKey)
        {
            item.SetTrigger("fadeOut");
        }
    }
}
