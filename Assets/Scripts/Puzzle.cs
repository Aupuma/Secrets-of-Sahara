using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Puzzle : NetworkBehaviour {

    public virtual void OnPuzzleReady()
    {

    }

    public virtual void PuzzleCompleted()
    {
        SceneObjectsManager.instance.HideObjects();
    }
}
