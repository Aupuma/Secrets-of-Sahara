using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Puzzle : NetworkBehaviour {

    public virtual void StartPuzzle()
    {

    }

    public virtual void PuzzleCompleted()
    {
        SceneObjectsManager.instance.HideObjects();
    }
}
