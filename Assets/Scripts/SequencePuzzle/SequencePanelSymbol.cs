using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencePanelSymbol : MonoBehaviour {

    private MeshRenderer textureRenderer;
    private Texture nextTexture;
    private Animator myAnimator;

    private void Start()
    {
        textureRenderer = GetComponent<MeshRenderer>();
        myAnimator = GetComponent<Animator>();
    }

    public void SetNewInfo(Texture texture)
    {
        nextTexture = texture;
        textureRenderer.material.SetTexture("_MainTex2", nextTexture);
        myAnimator.SetTrigger("CrossFade");
    }

    public void AssignNewTexture()
    {
        textureRenderer.material.SetTexture("_MainTex", nextTexture);
        textureRenderer.material.SetFloat("_Blend", 0f);
    }
}
