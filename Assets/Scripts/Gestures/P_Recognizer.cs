using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class P_Recognizer : MonoBehaviour {

    List<PDollarGestureRecognizer.Point> points = new List<PDollarGestureRecognizer.Point>();   // mouse points acquired from the user
    Gesture[] trainingSet = null;   // training set loaded from XML files
    GameObject clone;
    Text gestureText;

    void Start ()
    {
        trainingSet = LoadTrainingSet();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            clone = Instantiate(trailHolderPrefab, transform.position, Quaternion.identity) as GameObject;
            clone.transform.SetParent(transform);
            i++;
        }
        if (Input.GetMouseButton(0))
        {
            points.Add(new Point(Input.mousePosition.x, Input.mousePosition.y, i));

            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 pos = r.GetPoint(_distance);
            transform.position = pos;
        }
        if (Input.GetMouseButtonUp(0))
        {

            if (i >= 3)
            {
                i = 0;
                Point[] _points = points.ToArray();
                Gesture candidate = new Gesture(_points);
                string gestureCLass = PointCloudRecognizer.Classify(candidate, trainingSet);
                gestureText.text = gestureCLass;
            }
            clone.transform.parent = null;
            clone.GetComponent<TrailRenderer>().autodestruct = true;

            // GestureXML.GestureIO.WriteGesture(_points, "Triangle",Application.persistentDataPath+"test"+i.ToString()+".xml");
        }
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
            TextAsset textAsset = (TextAsset) file;
            gestures.Add(GestureIO.ReadGesture(textAsset.text));
        }
        return gestures.ToArray();
    }
}
