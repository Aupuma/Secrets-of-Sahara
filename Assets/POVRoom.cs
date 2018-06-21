using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVRoom : MonoBehaviour {

    public static POVRoom instance;

    [HideInInspector]
    public Animator animator;

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
