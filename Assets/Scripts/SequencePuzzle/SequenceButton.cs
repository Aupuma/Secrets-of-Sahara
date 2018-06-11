using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceButton : MonoBehaviour {

    Animator anim;
    MeshRenderer textureRenderer;
    public int id;
    private Texture nextTexture;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        textureRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
	}

    public void SetNewInfo(int n, Texture texture)
    {
        id = n;
        nextTexture = texture;
        textureRenderer.material.SetTexture("_Texture2", nextTexture);
        anim.SetTrigger("CrossFade");
    }

    public void AssignNewTexture()
    {
        textureRenderer.material.SetTexture("_MainTex", nextTexture);
        textureRenderer.material.SetFloat("_Blend", 0f);
    }

    public void ButtonPressed()
    {
        anim.SetTrigger("Pressed");
    }

    public void CheckIfButtonIsCorrect()
    {
        SequencePuzzleManager.instance.CmdOnButtonPressed(id);
    }
}
