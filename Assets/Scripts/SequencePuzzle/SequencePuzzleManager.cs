using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SequencePuzzleManager : NetworkBehaviour {

    public static SequencePuzzleManager instance;

    [Header("Parameters")]
    public int[] sequence;
    [SyncVar]
    private int seqIndex = 0;
    public bool isDebug = false;

    [Header("References")]
    public SequencePanelSymbol[] sequencePanelSymbols;
    public MeshRenderer[] sequencePanelBooleans;
    public Texture[] symbolTextures;
    public Material wrongBoolMaterial;
    public Material correctBoolMaterial;
    private SequenceButton[] buttons;
    private int[] btnIds;
    
    [Header("Scene objects with movement")]
    private Transform[] objectsRightRotation;
    private Transform[] objectsLeftRotation;
    private Transform[] objectsVerticalMovement;

    void Start () {
        instance = this;
        
        if(isServer)
        {
            if(isDebug) NetDiscovery.instance.StartAsServer();
            sequence = new int[4];
            btnIds = new int[15];
            buttons = FindObjectsOfType<SequenceButton>();
            //CmdGenerateNewSequence();
        }
    }

    private void StartSceneMovementLoop()
    {
        Sequence mySequence = DOTween.Sequence();

        Vector3 newRightRotation = new Vector3(0, 360, 0);
        for (int i = 0; i < objectsRightRotation.Length; i++)
        {
            if (i == 0) mySequence.Append(objectsRightRotation[i].DORotate(newRightRotation, 1f, RotateMode.FastBeyond360)).SetRelative();
            else mySequence.Join(objectsRightRotation[i].DORotate(newRightRotation, 1f, RotateMode.FastBeyond360)).SetRelative();
        }

        Vector3 newLeftRotation = new Vector3(0, -360, 0);
        for (int i = 0; i < objectsLeftRotation.Length; i++)
        {
            mySequence.Join(objectsLeftRotation[i].DORotate(newLeftRotation, 1f, RotateMode.FastBeyond360)).SetRelative();
        }

        for (int i = 0; i < objectsVerticalMovement.Length; i++)
        {
            mySequence.Join(objectsVerticalMovement[i].DOMoveY(objectsVerticalMovement[i].position.y - 1, 1f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.InSine));
        }

        mySequence.SetLoops(-1);
        mySequence.Play();
    }

    void Update()
    {
        // Code for OnMouseDown in the iPhone. Unquote to test.
        RaycastHit hit = new RaycastHit();
        if (Input.GetMouseButtonDown(0))
        {
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "SequenceButton")
                    hit.transform.gameObject.GetComponent<SequenceButton>().ButtonPressed();
            }
        }
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "SequenceButton")
                    hit.transform.gameObject.GetComponent<SequenceButton>().ButtonPressed();
            }
        }
    }


    public void GenerateRandomSequence(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            bool allowed = false;
            while (!allowed)
            {
                allowed = true;
                array[i] = UnityEngine.Random.Range(0, btnIds.Length);
                for (int j = 0; j < i; j++)
                {
                    if (array[j] == array[i])
                    {
                        allowed = false;
                        break;
                    }
                }
            }
        }
    }

    #region NETWORK METHODS

    [Command]
    public void CmdGenerateNewSequence()
    {
        Debug.Log("Generating new sequence");

        seqIndex = 0;

        RpcRestartSequence();

        GenerateRandomSequence(sequence);

        RpcAssignSymbolsToPanel(sequence);

        GenerateRandomSequence(btnIds);

        StartSceneMovementLoop();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetNewInfo(btnIds[i], symbolTextures[btnIds[i]]);
        }
    }

    [Command]
    public void CmdOnButtonPressed(int id)
    {
        //SI PULSAMOS BOTÓN CORRECTO LO HACEMOS SABER AL JUGADOR POV
        if (id == sequence[seqIndex])
        {
            RpcMarkSymbolAsCorrect();
            seqIndex += 1;
        }
        else //SI NO, REINICIAMOS LA SECUENCIA
        {
            CmdGenerateNewSequence();
            seqIndex = 0;
        }

        //SI SE PULSA TODA LA SECUENCIA CORRECTA, FINALIZAMOS PUZZLE
        if (seqIndex == sequence.Length)
        {
            seqIndex = 0;
            SceneObjectsManager.instance.HideObjects();
        }
    }

    [ClientRpc]
    private void RpcAssignSymbolsToPanel(int[] seq)
    {
        if (isServer == false)
        {
            for (int i = 0; i < seq.Length; i++)
            {
                sequencePanelSymbols[i].SetNewInfo(symbolTextures[seq[i]]);
            }
        }
    }

    [ClientRpc]
    private void RpcMarkSymbolAsCorrect()
    {
        if (isServer == false)
        {
            sequencePanelBooleans[seqIndex].material = correctBoolMaterial;
        }
    }

    [ClientRpc]
    private void RpcRestartSequence()
    {
        if (isServer == false)
        {
            foreach (var rendBool in sequencePanelBooleans)
            {
                rendBool.material = wrongBoolMaterial;
            }
        }
    } 

    #endregion //NETWORK METHODS
}
