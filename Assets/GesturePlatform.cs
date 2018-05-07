using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Enemy newEnemy = other.GetComponent<Enemy>();
            if(newEnemy.gestureType == gestureType)
            {
                enemiesInside.Add(newEnemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy oldEnemy = other.GetComponent<Enemy>();
            if (oldEnemy.gestureType == gestureType)
            {
                enemiesInside.Remove(oldEnemy);
            }
        }
    }

    public void DestroyEnemiesInside()
    {
        renderer.enabled = true;
        for (int i = enemiesInside.Count - 1; i >= 0; i--)
        {
            Enemy enemyToDestroy = enemiesInside[i];
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
