using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceButton : MonoBehaviour {

    Animator anim;
    public Image img;
    public int id;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}

    public void SetInfo(int n, Sprite spr)
    {
        id = n;
        img.sprite = spr;
    }

    public void ButtonPressed()
    {
        SequencePuzzleManager.instance.CmdOnButtonPressed(id);
        anim.SetTrigger("Pressed");
    }
}
