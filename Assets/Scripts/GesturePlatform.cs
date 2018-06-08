using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GesturePlatform : MonoBehaviour {

    public string gestureType;
    public List<Enemy> enemiesInside;
    private MeshRenderer renderer;
    
	// Use this for initialization
	void Start () {
        enemiesInside = new List<Enemy>();
        renderer = GetComponent<MeshRenderer>();
        renderer.enabled = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Enemy newEnemy = other.GetComponent<Enemy>();
            enemiesInside.Add(newEnemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy oldEnemy = other.GetComponent<Enemy>();
            EnemyManager.instance.OnEnemyFinishedPath(oldEnemy);
            enemiesInside.Remove(oldEnemy);
            oldEnemy.GetComponent<NavMeshAgent>().enabled = false;
            oldEnemy.gameObject.SetActive(false);
        }
    }

    public void GestureUsed()
    {
        renderer.enabled = true;
        for (int i = enemiesInside.Count - 1; i >= 0; i--)
        {
            Enemy enemyToDestroy = enemiesInside[i];
            EnemyManager.instance.OnGestureUsedInEnemy(enemyToDestroy);
            enemiesInside.RemoveAt(i);
            Destroy(enemyToDestroy.gameObject);
        }
        Invoke("FadeEffect", 1f);
    }

    public void FadeEffect()
    {
        renderer.enabled = false;
    }
}
