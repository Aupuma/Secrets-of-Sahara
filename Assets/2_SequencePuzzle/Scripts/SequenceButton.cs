using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceButton : MonoBehaviour {

    Animator anim;
    MeshRenderer textureRenderer;
    public int id;
    private Texture nextTexture;
    private AudioSource pushSound;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        textureRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        pushSound = GetComponent<AudioSource>();
	}

    public void SetNewInfo(int n, Texture texture)
    {
        id = n;
        nextTexture = texture;
        Debug.Log("Textura: " + texture);
        Debug.Log("Renderer: " + textureRenderer);
        textureRenderer.material.SetTexture("_MainTex2", nextTexture);
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
        pushSound.Play();
    }

    public void CheckIfButtonIsCorrect()
    {
        SequencePuzzleManager.instance.OnButtonPressed(id);
    }
}
