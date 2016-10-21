using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public GameObject self;
    float healthPoints = 1;
    public float speed = 2f;
    private Gun gun;
    private Character charSelf;
    private float height;
    private float moveRight;
    private float moveForward;
    // Use this for initialization
    void Start () {
        healthPoints = 100;
        gun = transform.GetComponentInChildren<Gun>();
        charSelf = transform.GetComponent<Character>();
        height = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        moveForward = 0;
        moveRight = 0;
		RaycastHit hit;
        bool playerInSight = false;
        float playerAngle = 0f;
        int numOfRayHits = 0;
        int targetHeight = 0;
        for (int angle = -40; angle < 40; angle+=2)
        {
            //cast rays at cover level
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    playerInSight = true;
                    numOfRayHits++;
                    playerAngle += angle;
                    targetHeight = 1;
                }
            }
        }
        for (int angle = -40; angle < 40; angle += 2)
        {
            //cast rays at head level
            if (Physics.Raycast(transform.position + new Vector3(0, 0.4f, 0) * (1 - transform.localScale.y) , Quaternion.Euler(0, angle, 0) * transform.forward, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    playerInSight = true;
                    numOfRayHits++;
                    playerAngle += angle;
                    targetHeight = 2;
                }
            }
        }
        if (playerInSight){
            if(targetHeight == 1)
            {
                charSelf.crouch();
            }
            else if(targetHeight == 2)
            {
                charSelf.uncrouch();
            }
            playerAngle = playerAngle / numOfRayHits;
            self.GetComponent<Renderer>().material.color = Color.green;
            transform.Rotate(0, playerAngle, 0);
            gun.fire();
        }else{
            charSelf.uncrouch();
            self.GetComponent<Renderer>().material.color = Color.red;
        }
        //----------------------------------------------debug enemy move
        if (Input.GetKey(KeyCode.T))
        {
            moveForward = 1;
        }else if (Input.GetKey(KeyCode.G))
        {
            moveForward = -1;
        }
        if (Input.GetKey(KeyCode.F))
        {
            moveRight = -1;
        }else if (Input.GetKey(KeyCode.H))
        {
            moveRight = 1;
        }
        //----------------------------------------------end of enemy move
        charSelf.move(moveRight, moveForward, speed);
        if (healthPoints < 50)
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
