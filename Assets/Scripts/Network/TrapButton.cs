using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapButton : MonoBehaviour {

    public static TrapButton instance;
    private Animator anim;
    
    // Use this for initialization
    void Start()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void ButtonPressed()
    {
        anim.SetTrigger("Pressed");
    }
}
