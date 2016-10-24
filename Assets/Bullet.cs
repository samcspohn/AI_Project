using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 3);
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Enemy")
        {
            Character enemy = collider.GetComponent<Character>();
            Debug.Log("hit: " + enemy);
            if(enemy != null)
            {
                enemy.TakeDamage(15f);
            }
        }else if(collider.tag == "Player")
        {
            Character player = collider.GetComponent<Character>();
            Debug.Log("hit: " + player);
            if (player != null)
            {
                player.TakeDamage(15f);
            }
        }
        Destroy(gameObject);
    }
}
