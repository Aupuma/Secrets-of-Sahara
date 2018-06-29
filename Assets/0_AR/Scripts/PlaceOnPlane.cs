using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    public GameObject arWorldOrigin;

    private List<ARRaycastHit> hits;
    private ARSessionOrigin sessionOrigin;
    private ARPlaneManager planeManager;
    private Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

    void Awake()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();
        planeManager = GetComponent<ARPlaneManager>();
        hits = new List<ARRaycastHit>();
    }

    private void Start()
    {
        arWorldOrigin = Instantiate(arWorldOrigin);
        arWorldOrigin.GetComponent<ARWorldOrigin>().sessionOrigin = sessionOrigin;
        arWorldOrigin.SetActive(false);
    }

    void Update()
    {
        //Mandamos raycast desde el centro de la pantalla a la escena para ver si colisiona con un plano
        if (sessionOrigin.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            //Si choca obtenemos la posición donde el rayo ha tocado el plano
            Pose hitPose = hits[0].pose;

            //Si no está activo el objeto origen lo activamos y colocamos en la posicion de choque
            if (!arWorldOrigin.activeSelf) arWorldOrigin.SetActive(true);
            arWorldOrigin.transform.position = hitPose.position;
            arWorldOrigin.transform.rotation = hitPose.rotation;

            //Si el usuario pulsa en la pantalla:
            if (Input.touchCount > 0)
            {
                ChoosePlane(hits[0].trackableId); //Elegimos el plano de juego
                arWorldOrigin.GetComponent<ARWorldOrigin>().HideUIandStartGame(); //Ocultamos la UI auxiliar del plano
                this.enabled = false; //Desactivamos este script
            }
        }
        else //Si no hay choque desactivamos el objeto origen
        {
            arWorldOrigin.SetActive(false);
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