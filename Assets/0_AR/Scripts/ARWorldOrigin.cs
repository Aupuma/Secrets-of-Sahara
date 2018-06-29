using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ARWorldOrigin : MonoBehaviour {

    public GameObject searchingUI;

    #region SINGLETON
    public static ARWorldOrigin instance;
    private void Awake()
    {
        instance = this;
    } 
    #endregion

    public void HideUI()
    {
        GetComponent<Animator>().SetTrigger("Placed");
    }

    public void DisableUI()
    {
        searchingUI.SetActive(false);
    }
}
