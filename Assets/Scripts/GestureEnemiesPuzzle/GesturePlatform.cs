using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GesturePlatform : MonoBehaviour {

    public string gestureType;
    private Enemy enemyInside;
    private MeshRenderer rendr;
    
	// Use this for initialization
	void Start () {
        rendr = GetComponent<MeshRenderer>();
        rendr.enabled = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemyInside = other.GetComponent<Enemy>();
        }
    }

    public void GestureUsed()
    {
        rendr.enabled = true;
        if (enemyInside != null)
        {
            EnemyManager.instance.OnGestureUsedInEnemy(enemyInside);
            enemyInside.DeactivateEnemy();
        }
        Invoke("FadeEffect", 1f);
    }

    public void FadeEffect()
    {
        rendr.enabled = false;
    }
}
