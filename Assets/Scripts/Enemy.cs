using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    SNAKE,
    BIRD,
    CAT,
    LION,
    SUPER
}

public class Enemy : MonoBehaviour {

    public EnemyType type;
    private NavMeshAgent agent;
    [HideInInspector] public Transform objective;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trap")
        {
            Destroy(gameObject);
        }
    }

}
