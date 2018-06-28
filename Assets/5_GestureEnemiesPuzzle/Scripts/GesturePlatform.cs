using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GesturePlatform : MonoBehaviour {

    public string gestureType;
    private Enemy enemyInside;
    private MeshRenderer rendr;
    private ParticleSystem pSystem;
    
	// Use this for initialization
	void Start () {
        pSystem = GetComponentInChildren<ParticleSystem>();

        //DEBUG
        //rendr = GetComponent<MeshRenderer>();
        //rendr.enabled = false;
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
        if (enemyInside != null)
        {
            EnemyManager.instance.OnGestureUsedInEnemy(enemyInside);
            enemyInside.DeactivateEnemy();
        }
        pSystem.Play();

        //DEBUG
        //rendr.enabled = true; 
        //Invoke("FadeEffect", 1f);
    }

    //DEBUG
    //public void FadeEffect()
    //{
    //    rendr.enabled = false;
    //}
}
