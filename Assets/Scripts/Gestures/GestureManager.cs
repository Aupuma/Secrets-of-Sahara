using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GestureResult
{
    public string name;
    public float score;

    public GestureResult(string n,float s)
    {
        name = n;
        score = s;
    }
}

public class GestureManager : MonoBehaviour
{
    public static GestureManager instance;

    List<Point> points = new List<Point>();   // mouse points acquired from the user
    Gesture[] trainingSet = null;   // training set loaded from XML files

    public  GameObject       trailPrefab; //Prefab de trazo
    private GameObject       thisTrail;   //Trazo actual que se está dibujando
    private Vector3          startPos;    //Posición inicial de trazo
    private Plane            objPlane;
    private bool             drawingTrail;
    private float            drawingStartingTime;
    private Transform drawingPos;

    void Start()
    {
        trainingSet = LoadTrainingSet();
        drawingTrail = false;
        drawingPos = Camera.main.transform.GetChild(0);
        objPlane = new Plane(Camera.main.transform.forward * -1, drawingPos.position);
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

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)     
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0))
        //else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            KeepDrawing();
        }
        else if (Input.GetMouseButtonUp(0))
        //else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            FinishDrawing();
        }
    }

    #region Drawing the trail
    private void StartDrawing()
    {
        points = new List<Point>();

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (objPlane.Raycast(mRay, out rayDistance))
            startPos = mRay.GetPoint(rayDistance);

        //Instanciamos trazo
        thisTrail = (GameObject)Instantiate(trailPrefab, startPos, Quaternion.identity);
        drawingTrail = true;
        drawingStartingTime = Time.unscaledTime;
    }

    public void KeepDrawing()
    {
        //Añadimos punto actual al conjunto
        points.Add(new Point(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0));

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (objPlane.Raycast(mRay, out rayDistance))
            thisTrail.transform.position = mRay.GetPoint(rayDistance);
    }

    private void FinishDrawing()
    {
        if (thisTrail != null && Vector3.Distance(thisTrail.transform.position, startPos) < 0.1 
            && Time.unscaledTime - drawingStartingTime < 0.5f)
        {
            DestroyTrail(); //Si el trazo es muy pequeño lo destruimos
        }
        else
        {
            CompareGesture();
            DestroyTrail();
        }
    }

    private void DestroyTrail()
    {
        Destroy(thisTrail);
        for (int i = points.Count - 1; i >= 0; i--)
        {
            //Eliminamos puntos generados por el trazo
            points.Remove(points[i]);
        }
        drawingTrail = false;
    }
    #endregion

    #region Getting the result
    public GestureResult RecognizeGesture()
    {
        Gesture candidate = new Gesture(points.ToArray());
        return PointCloudRecognizer.Classify(candidate, trainingSet);
    }

    public void CompareGesture()
    {
        GestureResult result = RecognizeGesture();
        if(result.score > 0.75f)
        {
        }
        //if (result.name == currentActivePanel.gestureName && result.score > 0.75f)
        //{
        //    currentActivePanel.ShowSolution();
        //}
    } 
    #endregion
}
