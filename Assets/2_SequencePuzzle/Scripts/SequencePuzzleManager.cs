using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SequencePuzzleManager : Puzzle {

    //Parameters
    private int[] sequence;
    [SyncVar]
    private int seqIndex = 0;

    [Header("References")]
    public SequencePanelSymbol[] sequencePanelSymbols;
    public Animator[] sequencePanelBooleans;
    public Texture[] symbolTextures;
    private SequenceButton[] buttons;
    private int[] btnIds;
    private AudioSource correctSound;
    
    [Header("Scene objects with rotation")]
    public Transform[] objectsRightRotation;
    public Transform[] objectsLeftRotation;

    #region SINGLETON
    public static SequencePuzzleManager instance;

    public void Awake()
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
            StartPillarsRotationLoop();
        }
        correctSound = GetComponent<AudioSource>();
    }

    public override void OnPuzzleReady()
    {
        base.OnPuzzleReady();
        if(isServer) GenerateNewSequence();
    }

    /// <summary>
    /// Anima de manera continua los objetos de la escena, rotación y movimiento en YOYO
    /// </summary>
    private void StartPillarsRotationLoop()
    {
        Sequence rotSeq = DOTween.Sequence();

        Vector3 newRightRotation = new Vector3(0, 360, 0);
        foreach (var item in objectsRightRotation)
        {
            Tween rot = item.DORotate(newRightRotation, 10, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            rotSeq.Join(rot);
        }

        Vector3 newLeftRotation = new Vector3(0, -360, 0);
        foreach (var item in objectsLeftRotation)
        {
            Tween rot = item.DORotate(newLeftRotation, 10, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            rotSeq.Join(rot);
        }
        rotSeq.SetLoops(-1);
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
            sequencePanelBooleans[seqIndex].SetBool("Enabled",true);
            correctSound.Play();
        }
    }

    [ClientRpc]
    private void RpcRestartSequence()
    {
        if (isServer == false)
        {
            foreach (var anim in sequencePanelBooleans)
            {
                anim.SetBool("Enabled", false);
            }
        }
    } 

    #endregion //NETWORK METHODS
}
