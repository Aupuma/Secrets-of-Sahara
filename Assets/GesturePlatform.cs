using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GesturePlatform : MonoBehaviour {

    public string gestureType;
    public List<Enemy> enemiesInside;

	// Use this for initialization
	void Start () {
        enemiesInside = new List<Enemy>();
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
        foreach (var enemy in enemiesInside)
        {
            Destroy(enemy);
        }
        enemiesInside.RemoveAll((o) => o == null);
    }
}
