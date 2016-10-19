using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public GameObject self;
    float healthPoints = 1;
    public float speed = 3f;
	// Use this for initialization
	void Start () {
        healthPoints = 100;
	}

    // Update is called once per frame
    void Update()
    {


		RaycastHit hit;
        bool playerInSight = false;
        for(int angle = -20; angle < 20; angle++)
        {
            if (Physics.Raycast (transform.position, Quaternion.Euler(0, angle, 0) * Vector3.forward, out hit) && hit.collider.tag.Equals("Player")) {
                if(hit.collider.tag == "Player")
                {
                    playerInSight = true;
                }

                //print ("Found an object - distance: " + hit.distance);
                //print(hit.collider.tag);
            }
        }
        if (playerInSight)
        {
            self.GetComponent<Renderer>().material.color = Color.green;
        }else
        {
            self.GetComponent<Renderer>().material.color = Color.red;
        }
		
        if(healthPoints < 50)
        {
            GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
            GameObject nearestCover = covers[0];
            Vector3 nearMinusSelf;
            Vector3 currMinusSelf;
            foreach (GameObject cover in covers)
            {
                currMinusSelf = cover.transform.position - transform.position;
                nearMinusSelf = nearestCover.transform.position - transform.position;
                if ( currMinusSelf.magnitude < nearMinusSelf.magnitude)
                {
                    nearestCover = cover;
                }
            }
            Vector3 moveTo = nearestCover.transform.position - transform.position;
            if(moveTo.magnitude > 0.2f)
            {
                transform.Translate(moveTo.normalized * speed * Time.deltaTime);
            }
        }
        if (healthPoints <= 0)
        {
            Destroy(self);
        }
    }

    public void TakeDamage(float damage) {
        healthPoints -= damage;
    }
}
