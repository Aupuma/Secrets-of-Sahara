using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingPanel : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler {

    public static DrawingPanel instance;

    public bool pointerOnPanel = false;

    // Use this for initialization
    void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(pointerOnPanel);
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerOnPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOnPanel = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerOnPanel = false;
    }
}
