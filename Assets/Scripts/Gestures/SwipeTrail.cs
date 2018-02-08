using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using UnityEngine.UI;

public class SwipeTrail : MonoBehaviour
{
    List<Point> points = new List<Point>();   // mouse points acquired from the user
    Gesture[] trainingSet = null;   // training set loaded from XML files

    public  Text             gestureText; //Texto donde se mostrará el tipo de gesto dibujado
    public  GameObject       trailPrefab; //Prefab de trazo
    private GameObject       thisTrail;   //Trazo actual que se está dibujando
    private List<GameObject> drawingTrails = new List<GameObject>();
    private Vector3          startPos;    //Posición inicial de trazo
    private int              strokeID;    //Número de trazo
    private Plane            objPlane;

    void Start()
    {
        objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
        trainingSet = LoadTrainingSet();
        strokeID = -1;
    }

    void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
            Input.GetMouseButtonDown(0))
        {
            Debug.Log("Cuenta trazos: " + drawingTrails.Count + ", id trazo: " + strokeID);
            if (drawingTrails.Count > 0 && strokeID == -1) ResetDrawing();

            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
                startPos = mRay.GetPoint(rayDistance);

            //Instanciamos trazo
            thisTrail = (GameObject)Instantiate(trailPrefab, startPos, Quaternion.identity);
            drawingTrails.Add(thisTrail);
            strokeID++;
        }
        else if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            || Input.GetMouseButton(0)))
        {
            //Añadimos punto actual al conjunto
            points.Add(new Point(Input.mousePosition.x, Input.mousePosition.y, strokeID));

            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
                thisTrail.transform.position = mRay.GetPoint(rayDistance);
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) ||
         Input.GetMouseButtonUp(0))
        {
            //Si el trazo es muy pequeño lo destruimos
            if (Vector3.Distance(thisTrail.transform.position, startPos) < 0.1)
            {
                Destroy(thisTrail);
                drawingTrails.Remove(thisTrail);
                strokeID--;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RecognizeGesture();
        }
    }

    public void RecognizeGesture()
    {
        if(strokeID > -1)
        {
            strokeID = -1;
            Gesture candidate = new Gesture(points.ToArray());
            string gestureCLass = PointCloudRecognizer.Classify(candidate, trainingSet);
            gestureText.text = gestureCLass;
        }
    }

    private void ResetDrawing()
    {
        Debug.Log("Reiniciado");
        for (int i = drawingTrails.Count - 1; i >= 0; i--)
        {
            GameObject tempObject = drawingTrails[i];
            drawingTrails.Remove(drawingTrails[i]);
            Destroy(tempObject);
        }
        points = new List<Point>();
    }

    /// <summary>
    /// Loads training gesture samples from XML files
    /// </summary>
    /// <returns></returns>
    private Gesture[] LoadTrainingSet()
    {
        List<Gesture> gestures = new List<Gesture>();
        Object[] gestureFiles = Resources.LoadAll("Gestures");
        foreach (Object file in gestureFiles)
        {
            TextAsset textAsset = (TextAsset)file;
            gestures.Add(GestureIO.ReadGesture(textAsset));
        }
        return gestures.ToArray();
    }
}
