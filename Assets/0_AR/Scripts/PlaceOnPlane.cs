using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    public GameObject PlacedPrefab;

    private List<ARRaycastHit> hits;
    private GameObject spawnedObject;
    private ARSessionOrigin sessionOrigin;
    private ARPlaneManager planeManager;

    void Awake()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();
        planeManager = GetComponent<ARPlaneManager>();
        hits = new List<ARRaycastHit>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (sessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    ChoosePlane(hits[0].trackableId);

                    spawnedObject = Instantiate(PlacedPrefab, hitPose.position, hitPose.rotation);

                    this.enabled = false;
                }
            }
        }
    }

    private void ChoosePlane(TrackableId id)
    {
        ARPlane planeChosen = planeManager.TryGetPlane(id);
        List<ARPlane> arPlanes = new List<ARPlane>();
        planeManager.GetAllPlanes(arPlanes);

        //Eliminamos el resto de planos que no se van a usar
        for (int i = arPlanes.Count - 1; i == 0; i++)
        {
            ARPlane tempPlane = arPlanes[i];
            if (tempPlane != planeChosen)
            {
                arPlanes.Remove(tempPlane);
                Destroy(tempPlane);
            }
        }

        planeManager.enabled = false;
    }
}