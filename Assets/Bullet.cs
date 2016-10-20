using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public GameObject self;
    public GameObject Enemy;
    public float selfDestructTimer = 300;
    private float selfDestructNow;
	// Use this for initialization
	void Start () {
        selfDestructNow = Time.time + selfDestructTimer;
	}
	
	// Update is called once per frame
	void Update () {
	    if(Time.time > selfDestructNow)
        {
            Destroy(self);
        }
	}
    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Enemy")
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            Debug.Log("hit: " + enemy);
            if(enemy != null)
            {
                enemy.TakeDamage(15f);
            }
        }else if(collider.tag == "Player")
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            Debug.Log("hit: " + enemy);
            if (enemy != null)
            {
                enemy.TakeDamage(15f);
            }
        }
        Destroy(self);
    }
}
