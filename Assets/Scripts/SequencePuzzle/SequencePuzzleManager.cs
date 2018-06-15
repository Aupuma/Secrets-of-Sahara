using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SequencePuzzleManager : Puzzle {

    [Header("Parameters")]
    public int[] sequence;
    [SyncVar]
    private int seqIndex = 0;

    [Header("References")]
    public SequencePanelSymbol[] sequencePanelSymbols;
    public MeshRenderer[] sequencePanelBooleans;
    public Texture[] symbolTextures;
    public Material wrongBoolMaterial;
    public Material correctBoolMaterial;
    private SequenceButton[] buttons;
    private int[] btnIds;
    
    [Header("Scene objects with movement")]
    public Transform[] objectsRightRotation;
    public Transform[] objectsLeftRotation;
    public Transform totem1Buttons;
    public Transform totem2Buttons;

    #region SINGLETON
    public static SequencePuzzleManager instance;

    private void Awake()
    {
        instance = this;
    } 
    #endregion //SINGLETON

    public override void Start () {
        base.Start();

        if(isServer)
        {
            sequence = new int[4];
            btnIds = new int[15];
            buttons = FindObjectsOfType<SequenceButton>();
            StartSceneMovementLoop();
        }
    }

    public override void OnPuzzleReady()
    {
        if (isServer) GenerateNewSequence();
        base.OnPuzzleReady();
    }

    /// <summary>
    /// Anima de manera continua los objetos de la escena, rotación y movimiento en YOYO
    /// </summary>
    private void StartSceneMovementLoop()
    {
        Sequence run = DOTween.Sequence();

        Vector3 newRightRotation = new Vector3(0, 360, 0);
        foreach (var item in objectsRightRotation)
        {
            Tween rot = item.DORotate(newRightRotation, 10, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            run.Join(rot);
        }

        Vector3 newLeftRotation = new Vector3(0, -360, 0);
        foreach (var item in objectsLeftRotation)
        {
            Tween rot = item.DORotate(newLeftRotation, 10, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            run.Join(rot);
        }
        run.SetLoops(-1);

        totem1Buttons.DOLocalMoveY(totem1Buttons.position.y - 1, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
        totem2Buttons.DOLocalMoveY(totem2Buttons.position.y + 1, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
    }

    void Update()
    {
        //if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "SequenceButton")
                    hit.collider.GetComponent<SequenceButton>().ButtonPressed();
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

    public void GenerateNewSequence()
    {
        Debug.Log("Generating new sequence");

        seqIndex = 0;

        RpcRestartSequence();

        GenerateRandomSequence(sequence);

        RpcAssignSymbolsToPanel(sequence);

        GenerateRandomSequence(btnIds);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetNewInfo(btnIds[i], symbolTextures[btnIds[i]]);
        }
    }

    public void OnButtonPressed(int id)
    {
        //SI PULSAMOS BOTÓN CORRECTO LO HACEMOS SABER AL JUGADOR POV
        if (id == sequence[seqIndex])
        {
            RpcMarkSymbolAsCorrect();
            seqIndex += 1;
        }
        else //SI NO, REINICIAMOS LA SECUENCIA
        {
            GenerateNewSequence();
            seqIndex = 0;
        }

        //SI SE PULSA TODA LA SECUENCIA CORRECTA, FINALIZAMOS PUZZLE
        if (seqIndex == sequence.Length)
        {
            seqIndex = 0;
            PuzzleCompleted();
        }
    }

    #region NETWORK METHODS

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
