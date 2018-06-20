using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Puzzle : NetworkBehaviour {

    public bool isDebug = false;

    [Header ("Player dependent objects")]
    public GameObject[] AR_Player_Objects;
    public GameObject[] POV_Player_Objects;

    private bool hasStarted = false;
    [HideInInspector]
    public Animator animator;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        HidePlayerDependentObjects();
        
        if (isServer)
        {
            this.transform.position = ARWorldOrigin.instance.transform.position;
            //this.transform.rotation = ARWorldOrigin.instance.transform.rotation;
        }
    }

    public override void OnStartServer()
    {
        if (isDebug) NetDiscovery.instance.StartAsServer();
    }

    public void HidePlayerDependentObjects()
    {
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

    /// <summary>
    /// Llamado desde el final de la animación de aparecer escenario
    /// </summary>
    public virtual void OnPuzzleReady()
    {
        hasStarted = true;
        //if (isDebug) NetDiscovery.instance.StartAsServer();
    }

    /// <summary>
    /// Llamado una vez se ha completado el puzzle, activa animación de
    /// desaparición del escenario
    /// </summary>
    public virtual void PuzzleCompleted()
    {
        animator.SetTrigger("Disappear");
    }

    /// <summary>
    /// Llamado desde el final de la animacion de desaparecer escenario
    /// </summary>
    public void LoadNextLevel()
    {
        if (hasStarted) GameManager.instance.LoadNextScene();
    }
}
