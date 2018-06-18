using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    DOG,
    BIRD,
    CAT,
    FOX,
    WOLF
}

public class Enemy : MonoBehaviour {

    public EnemyType type;
    private NavMeshAgent agent;
    private Animator animator;
    [HideInInspector] public Transform objective;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trap")
        {
            DeactivateEnemy();
        }
        else if (other.tag == "ExitPortal")
        {
            EnemyManager.instance.OnEnemyFinishedPath(this);
            DeactivateEnemy();
        }
    }

    public void PlayDisappearAnimation()
    {
        animator.SetTrigger("Dissolve");
    }

    public void DeactivateEnemy()
    {
        agent.enabled = false;
        gameObject.SetActive(false);
    }
}
