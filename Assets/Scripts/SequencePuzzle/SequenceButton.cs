using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceButton : MonoBehaviour {

    Animator anim;
    MeshRenderer textureRenderer;
    public int id;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        textureRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
	}

    public void SetInfo(int n, Material mat)
    {
        id = n;
        textureRenderer.material = mat;
    }

    public void ButtonPressed()
    {
        SequencePuzzleManager.instance.CmdOnButtonPressed(id);
        anim.SetTrigger("Pressed");
    }
}
