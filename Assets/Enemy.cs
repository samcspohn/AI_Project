using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public GameObject self;
    public GameObject debugObject;
    public Transform arbitraryCube;
    private GameObject debugObj;
    public float speed = 2f;
    public float turn = 0.5f;
    private Gun gun;
    private Character charSelf;
    private Transform closestEdge;
    public static List<GameObject> debugObjects;
    private static List<edgeType> debugObjectsType;
    private float currTime = 0;
    private float height;
    private float moveRight;
    private float moveForward;
    private enum edgeType{convex, concave};
    private char action = ' ';
    private float randAngleMult;
    // Use this for initialization
    void Start () {
        debugObjects = new List<GameObject>();
        debugObjectsType = new List<edgeType>();
        gun = transform.GetComponentInChildren<Gun>();
        charSelf = transform.GetComponent<Character>();
        charSelf.healthPoints = 100;
        height = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        while(debugObjects.Count > 0){
                DestroyImmediate(debugObjects[0]);
                debugObjects.RemoveAt(0);
                debugObjectsType.RemoveAt(0);
        }
        moveForward = 0;
        moveRight = 0;
		RaycastHit hit1;
        RaycastHit hit2 = new RaycastHit();
        RaycastHit hit3 = new RaycastHit();
        RaycastHit hit0 = new RaycastHit(); // this is used as an assuring edge/corner detector -- is not cast unless preliminary requirements are met
        // = new GameObject();
        
        bool playerInSight = false;
        float playerAngle = 0f;
        int numOfRayHits = 0;
        int targetHeight = 0;
        int numOfCasts = 0;
        float slope = -1;
        float lastSlope = -1f;
        float slope3 = -1f;
        bool lastSlopeDiff = false;
        closestEdge = arbitraryCube;
        for (int angle = -60; angle < 60; angle+=1)
        {
            bool isEdge = false;
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
                                
                                if (hit1.collider.transform.GetInstanceID() != hit2.collider.transform.GetInstanceID())// definitely something
                                {
                                    if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle + 1, 0) * transform.forward, out hit0))
                                    {
                                        //check if too close - later


                                        if(hit1.distance < hit2.distance)//something different is closer
                                        {
                                            if ((hit2.distance + hit3.distance) / 2 - (hit1.distance + hit0.distance) / 2  > 0.5 * Mathf.Pow((Mathf.Abs(hit0.distance - hit1.distance) / (hit0.distance * 0.01744f)), 0.5f) || (hit1.distance + hit2.distance) < (hit0.distance + hit3.distance))
                                            {
                                                debugObjects.Add(Instantiate(debugObject, hit1.point, Quaternion.identity) as GameObject);//is it an edge "convex"
                                                debugObjectsType.Add(edgeType.convex);
                                                isEdge = true;
                                            }
                                            else
                                            {
                                                debugObjects.Add(Instantiate(debugObject, hit2.point, Quaternion.identity) as GameObject);//or a corner "concave"
                                                debugObjectsType.Add(edgeType.concave);
                                                debugObj = debugObjects[debugObjects.Count - 1];
                                                debugObj.GetComponent<Renderer>().material.color = new Color(0, 1, 0.7f);
                                            }
                                        }
                                        else //something different is farther
                                        {

                                            if ((hit1.distance + hit0.distance) / 2 - (hit2.distance + hit3.distance) / 2 > 0.5 * Mathf.Pow((Mathf.Abs(hit2.distance - hit3.distance) / (hit3.distance * 0.01744f)), 0.5f) || (hit1.distance + hit2.distance) < (hit0.distance + hit3.distance)) // edge corner
                                            {
                                                debugObjects.Add(Instantiate(debugObject, hit2.point, Quaternion.identity) as GameObject);//is it an edge "convex"
                                                debugObjectsType.Add(edgeType.convex);
                                                isEdge = true;
                                            }
                                            else
                                            {
                                                debugObjects.Add(Instantiate(debugObject, hit1.point, Quaternion.identity) as GameObject);//or a corner "concave"
                                                debugObjectsType.Add(edgeType.concave);
                                                debugObj = debugObjects[debugObjects.Count - 1];
                                                debugObj.GetComponent<Renderer>().material.color = new Color(0, 1, 0.7f);
                                            }

                                        }
                                    }
                                    if((debugObjects[debugObjects.Count - 1].transform.position - transform.position).magnitude < (closestEdge.position - transform.position).magnitude && isEdge)
                                    {
                                        closestEdge = debugObjects[debugObjects.Count - 1].transform;
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
            transform.GetComponent<Renderer>().material.color = Color.green;
            transform.Rotate(0, playerAngle, 0);
            gun.fire();
        }else{
            charSelf.uncrouch();
            transform.GetComponent<Renderer>().material.color = Color.red;
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

        //automatic movement
        //input
        
        if(Time.time > currTime + turn){
            bool breaker = false;
            foreach(KeyCode input in System.Enum.GetValues(typeof(KeyCode))){
                if(Input.GetKey(input)){
                    breaker = true;
                    switch(input){          
                        case KeyCode.P:
                            Debug.Log("P");
                            action = 'p';
                            currTime = Time.time;
                            break;
                        case KeyCode.O:
                            Debug.Log("O");
                            action = 'o';
                            randAngleMult = Random.value * 2 - 1;
                            currTime = Time.time;
                            break;
                        default:
                            action = ' ';
                            breaker = false;
                            break;
                    }
                }
                if(breaker)
                    break;
            }
        }
        
        //output
        
        if (Time.time < currTime + turn)// move to edge in a turn
        {
            if(action == 'p' && closestEdge.GetInstanceID() != arbitraryCube.GetInstanceID())
                charSelf.move((closestEdge.position - transform.position), speed);
            else if(action == 'o')
                patrol(randAngleMult);
        }

        

        //----------------------------------------------end of enemy move
        charSelf.move(moveRight, moveForward, speed);
        // hide behind cover
        if (charSelf.healthPoints <= 0)
        {
            Destroy(self);
        }
        transform.rotation.eulerAngles.Set(0, transform.rotation.eulerAngles.y, 0);
    }
    void patrol(float randAngleMult){

        
        RaycastHit ray;
        float[] wallDists = {0,0,0};
        Vector3[] wallHits = {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
        //int i = 0;
        for(int angle = 0; angle < 8; angle++){
            if(Physics.Raycast(transform.position, Quaternion.Euler(0,(angle - 4) * 45,0) * transform.forward, out ray)){
                wallHits[angle] = ray.point;
                //i++;
            }
        }
        // if(Physics.Raycast(transform.position, transform.right, out ray)){
        //     wallDists[0] = ray.distance;
        //     wallHits[0] = ray.point;
        // }
        // if(Physics.Raycast(transform.position, transform.forward, out ray)){
        //     wallDists[1] = ray.distance;
        //     wallHits[1] = ray.point;
        // }
        // if(Physics.Raycast(transform.position, -transform.right, out ray)){
        //     wallDists[2] = ray.distance;
        //     wallHits[2] = ray.point;
        // }
        // int min = 0;
        // int max = 0;
        //find the minimum and maximum distance vector of the three directions
        // for(int i = 0; i < 3; i++){
        //     if(wallDists[min] < wallDists[i]){
        //         min = i;
        //     }
        //     if(wallDists[max] > wallDists[i]){
        //         max = i;
        //     }
        // }
        Vector3 averageAvoidPosition = (transform.position);
        List<int> indeces = new List<int>();
        int index = 0;
        foreach(edgeType thisEdge in debugObjectsType){
            if(thisEdge == edgeType.concave)
                indeces.Add(index);
                index++;
        }
        foreach(int index_ in indeces){
            if((debugObjects[index_].GetComponent<Transform>().position - transform.position).magnitude < 3)
                averageAvoidPosition -= debugObjects[index_].GetComponent<Transform>().position;// / (debugObjects[index_].GetComponent<Transform>().position - transform.position).magnitude;
        }
        foreach(Vector3 wallHit in wallHits){
            if((wallHit - transform.position).magnitude < 3)
                averageAvoidPosition += wallHit;// / (wallHit - transform.position).magnitude;
        }
        averageAvoidPosition.y = 0;
        GameObject debugObj = Instantiate(debugObject, averageAvoidPosition, transform.rotation) as GameObject;
        debugObj.GetComponent<Renderer>().material.color = new Color(1,0,0);
        debugObj.transform.localScale.Scale(new Vector3(10,10,10));
        debugObjects.Add(debugObj);
        debugObjectsType.Add(edgeType.convex);
        //avoid walls and concave corners
        //charSelf.turn(60 * randAngleMult * Time.deltaTime * Mathf.Clamp((averageAvoidPosition - transform.position).magnitude, 0, 1));
        //if((averageAvoidPosition - transform.position).magnitude < 1.5)
        charSelf.turn(averageAvoidPosition, 2f * Time.deltaTime);// / (averageAvoidPosition - transform.position).magnitude);
        
        // if(wallDists[min] < 1.3){
        //     charSelf.turn(-averageAvoidPosition, 300 * Time.deltaTime);
        //     // if(min == 1)
        //     //     charSelf.turn(360 * Time.deltaTime);
        //     // else
        //     //     charSelf.turn(((min-1) * 120 / wallDists[min]) * Time.deltaTime);
        // }else{
        //     charSelf.turn(60 * randAngleMult * Time.deltaTime);
        // }
        charSelf.move(transform.forward, 2f);
    }
}
