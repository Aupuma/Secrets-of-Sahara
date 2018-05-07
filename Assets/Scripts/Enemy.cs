using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public string gestureType;
    private NavMeshAgent agent;

    [HideInInspector]
    public Transform objective;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        //agent.SetDestination(objective.transform.position);
    }

    // Update is called once per frame
    void Update () {
		
	}

}
