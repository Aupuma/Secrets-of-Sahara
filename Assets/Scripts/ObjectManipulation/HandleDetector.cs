using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleDetector : MonoBehaviour {

    private Draggable currentDraggedObject;
	
	// Update is called once per frame
	void Update () {
        if(currentDraggedObject != null) //Estamos moviendo un objeto
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentDraggedObject.Selected = false;
                currentDraggedObject = null;
            }
        }
        else //Lanzamos raycast para saber si estamos tocando un handle
        {
            RaycastHit hit = new RaycastHit();
            if (Input.GetMouseButtonDown(0))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Handle")
                    {
                        currentDraggedObject = hit.transform.GetComponentInParent<Draggable>();
                        currentDraggedObject.Selected = true;
                    }
                }
            }
            /*
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Handle")
                    {
                        currentHandle = hit.transform.gameObject.GetComponent<MoveParentObj>();
                        currentHandle.selected = true;
                    }
                }
            }*/
        }
    }
}
