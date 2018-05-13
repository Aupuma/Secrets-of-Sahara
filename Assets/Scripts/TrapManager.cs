using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour {

    public static TrapManager instance;
    public GameObject[] traps;

    private void Start()
    {
        instance = this;
    }

    public void TrapsOnOff()
    {
        foreach (var t in traps)
        {
            if (!t.activeSelf) t.SetActive(true);
            else t.SetActive(false);
        }
    }
}
