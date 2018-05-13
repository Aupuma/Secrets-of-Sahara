using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SequencePuzzleManager : NetworkBehaviour {

    public static SequencePuzzleManager instance;

    //Data
    public int[] sequence;
    [SyncVar]
    public int seqIndex = 0;
    public bool solved = false;
    public bool isDebug = false;

    //References
    private SequenceButton[] buttons;
    public int[] btnIds;
    public Sprite[] symbolTextures;
    public GameObject sequencePanel;
    public Image[] sequencePanelSymbols;
    public Image[] sequencePanelBooleans;

	void Start () {
        instance = this;

        if (isServer == false)
            sequencePanel.SetActive(true);
        else
        {
            if(isDebug) NetDiscovery.instance.StartAsServer();
            sequence = new int[4];
            btnIds = new int[15];
            buttons = FindObjectsOfType<SequenceButton>();
        }
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

    [Command]
    public void CmdGenerateNewSequence()
    {
        seqIndex = 0;

        RpcRestartSequence();

        GenerateRandomSequence(sequence);

        RpcAssignSymbolsToPanel(sequence);

        GenerateRandomSequence(btnIds);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetInfo(btnIds[i], symbolTextures[btnIds[i]]);
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

    [Command]
    public void CmdOnButtonPressed(int id)
    {
        if(id == sequence[seqIndex])
        {
            RpcMarkSymbolAsCorrect();
            seqIndex += 1;
        }
        else
        {
            RpcRestartSequence();
            seqIndex = 0;
        }

        if(seqIndex == sequence.Length)
        {
            solved = true;
            seqIndex = 0;
        }
    }

    [ClientRpc]
    private void RpcAssignSymbolsToPanel(int[] seq)
    {
        Debug.Log(seq);
        if (isServer == false)
        {
            for (int i = 0; i < seq.Length; i++)
            {
                sequencePanelSymbols[i].sprite = symbolTextures[seq[i]];
            }
        }
    }

    [ClientRpc]
    private void RpcMarkSymbolAsCorrect()
    {
        if (isServer == false)
        {
            sequencePanelBooleans[seqIndex].color = Color.green;
        }
    }

    [ClientRpc]
    private void RpcRestartSequence()
    {
        if (isServer == false)
        {
            foreach (var imgBool in sequencePanelBooleans)
            {
                imgBool.color = Color.red;
            }
        }
    }
}
