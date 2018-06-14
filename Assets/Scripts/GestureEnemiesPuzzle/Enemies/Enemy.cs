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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trap")
        {
            gameObject.SetActive(false);
        }
    }

    public void DeactivateEnemy()
    {
        gameObject.SetActive(false);
    }
}
