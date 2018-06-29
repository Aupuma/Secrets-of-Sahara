using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

public class ARWorldOrigin : MonoBehaviour {

    public GameObject searchingUI;

    [HideInInspector]
    public ARSessionOrigin sessionOrigin;

    #region SINGLETON
    public static ARWorldOrigin instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public void HideUIandStartGame()
    {
        GetComponent<Animator>().SetTrigger("Placed");
        NetworkManager.singleton.StartHost(); //Empezamos el juego como servidor
    }

    public void DisableUI()
    {
        searchingUI.SetActive(false);
    }

    public void PlaceLevelAtOrigin(Transform lvlTransform)
    {
        sessionOrigin.MakeContentAppearAt(lvlTransform, this.transform.position, this.transform.rotation);
        transform.position = lvlTransform.position;
        transform.rotation = lvlTransform.rotation;
    }
}
