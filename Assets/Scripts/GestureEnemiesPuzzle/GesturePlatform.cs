using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GesturePlatform : MonoBehaviour {

    public string gestureType;
    public List<Enemy> enemiesInside;
    private MeshRenderer rendr;
    
	// Use this for initialization
	void Start () {
        enemiesInside = new List<Enemy>();
        rendr = GetComponent<MeshRenderer>();
        rendr.enabled = false;
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
            enemiesInside.Remove(oldEnemy);
        }
    }

    public void GestureUsed()
    {
        rendr.enabled = true;
        for (int i = enemiesInside.Count - 1; i >= 0; i--)
        {
            Enemy enemyToDestroy = enemiesInside[i];
            EnemyManager.instance.OnGestureUsedInEnemy(enemyToDestroy);
            enemiesInside.RemoveAt(i);
            if (enemyToDestroy.type != EnemyType.SUPER)
            {
                enemyToDestroy.PlayDisappearAnimation();
            }
        }
        Invoke("FadeEffect", 1f);
    }

    public void FadeEffect()
    {
        rendr.enabled = false;
    }
}
