using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SceneObjectsManager : NetworkBehaviour {

    public static SceneObjectsManager instance;

    public GameObject[] AR_Player_Objects;
    public GameObject[] POV_Player_Objects;
    public GameObject puzzle;
    private bool hasStarted = false;
    private Animator animator;

    // Use this for initialization
    void Start () {
        instance = this;
        animator = GetComponent<Animator>();

        if (isServer) //Somos el jugador en AR
        {
            foreach (var obj in POV_Player_Objects)
            {
                obj.SetActive(false);
            }
        }
        else //Somos el jugador en primera persona
        {
            foreach (var obj in AR_Player_Objects)
            {
                obj.SetActive(false);
            }
        }
	}

    public void StartPuzzle()
    {
        SequencePuzzleManager.instance.CmdGenerateNewSequence();
        hasStarted = true;
    }

    public void HideObjects()
    {
        animator.SetTrigger("Disappear");
    }

    public void LoadNextLevel()
    {
        if (hasStarted) GameManager.instance.LoadNextScene();
    }
}
