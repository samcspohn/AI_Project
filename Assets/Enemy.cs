using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public GameObject self;
    public GameObject debugObject;
    public float speed = 2f;
    private Gun gun;
    private Character charSelf;
    private float height;
    private float moveRight;
    private float moveForward;
    // Use this for initialization
    void Start () {      
        gun = transform.GetComponentInChildren<Gun>();
        charSelf = transform.GetComponent<Character>();
        charSelf.healthPoints = 100;
        height = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        moveForward = 0;
        moveRight = 0;
		RaycastHit hit1;
        RaycastHit hit2 = new RaycastHit();
        RaycastHit hit3 = new RaycastHit();
        RaycastHit hit0 = new RaycastHit(); // this is used as an assuring edge/corner detector -- is not czst unless preliminary requirements are met
        bool playerInSight = false;
        float playerAngle = 0f;
        int numOfRayHits = 0;
        int targetHeight = 0;
        int numOfCasts = 0;
        float slope = -1;
        float lastSlope = -1f;
        float slope3 = -1f;
        bool lastSlopeDiff = false;
        for (int angle = -60; angle < 60; angle+=1)
        {
            //cast rays at cover level
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward, out hit1))
            {
                if (hit1.collider.tag == "Player")
                {
                    playerInSight = true;
                    numOfRayHits++;
                    playerAngle += angle;
                    targetHeight = 1;
                }else//did not hit player - calculate object edge location
                {
                    if (numOfCasts > 1)
                    {
                        slope = (hit1.point.z - hit2.point.z) / (hit1.point.x - hit2.point.x);
                        if (numOfCasts > 2)
                        {
                            //slope = (hit1.point.z - hit2.point.z) / (hit1.point.x - hit2.point.x);
                            if (numOfCasts > 3)
                            {
                                GameObject debugObj = new GameObject();
                                if (hit1.collider.transform.GetInstanceID() != hit2.collider.transform.GetInstanceID())// && (hit1.collider.transform.rotation.y == hit3.collider.transform.rotation.y && Mathf.Abs(hit1.distance - hit3.distance) > 1))//detect if edge
                                {
                                    if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle + 1, 0) * transform.forward, out hit0))
                                    {
                                        if ((hit3.distance + hit0.distance) < (hit1.distance + hit2.distance) && Mathf.Abs(hit1.distance - hit2.distance) < 0.6)//this is a corner
                                        {
                                            if (hit1.distance < hit2.distance)
                                                debugObj = Instantiate(debugObject, hit1.point, Quaternion.identity) as GameObject;
                                            else
                                                debugObj = Instantiate(debugObject, hit2.point, Quaternion.identity) as GameObject;
                                            //lastSlopeDiff = true;
                                            debugObj.GetComponent<Renderer>().material.color = new Color(0, 1, 0.7f);
                                        }
                                        else //if(hit2.distance > (hit1.distance + hit3.distance) / 2)//this is an edge
                                        {
                                            if (hit1.distance < hit2.distance)
                                                debugObj = Instantiate(debugObject, hit1.point, Quaternion.identity) as GameObject;
                                            else
                                                debugObj = Instantiate(debugObject, hit2.point, Quaternion.identity) as GameObject;
                                            //lastSlopeDiff = true;
                                        }
                                    }
                                }
                            }
                            slope3 = lastSlope;
                            hit3 = hit2;
                        }
                        lastSlope = slope;
                    }
                }
                hit2 = hit1;
                numOfCasts++;
            }
        }
        for (int angle = -60; angle < 60; angle += 1)
        {
            //cast rays at head level
            if (Physics.Raycast(transform.position + new Vector3(0, 0.4f, 0) * (1 - transform.localScale.y) , Quaternion.Euler(0, angle, 0) * transform.forward, out hit1))
            {
                if (hit1.collider.tag == "Player")
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
        var camRotateHorizontal = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 80;
        charSelf.turn(camRotateHorizontal);
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
        /*
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
        }*/
        if (charSelf.healthPoints <= 0)
        {
            Destroy(self);
        }
    }

   /* public void TakeDamage(float damage) {
        healthPoints -= damage;
    }*/
}
