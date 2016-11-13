using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public GameObject self;
    public GameObject debugObject;
    public GameObject areaAvoid;
    public Transform arbitraryCube;
    private GameObject debugObj;

    public float speed = 2f;
    public float turn = 0.5f;
    public float avoidAreaRate = 2;

    private Gun gun;
    private Character charSelf;
    private Transform closestEdge;

    public static List<GameObject> debugObjects;
    private static List<edgeType> debugObjectsType;

    private float currTime = 0;
    private float currTimeA = 0;
    private float height;
    private float moveRight;
    private float moveForward;
    private enum edgeType{convex, concave};
    private char action = ' ';
    private float randAngleMult;

    int avoid;
    int avoidMask;


    // Use this for initialization
    void Start () {
        debugObjects = new List<GameObject>();
        debugObjectsType = new List<edgeType>();
        gun = transform.GetComponentInChildren<Gun>();
        charSelf = transform.GetComponent<Character>();
        charSelf.healthPoints = 100;
        height = transform.localScale.y;
        currTimeA = Time.time;

        avoid = 1 << 4;
        avoidMask = ~0 & avoid;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;  i < debugObjects.Count;){
            if(debugObjects[i] != null && debugObjects[i].GetComponent<AvoidArea>() != null)
            {
                if(debugObjects[i].GetComponent<AvoidArea>().RemoveTimer < Time.time)
                {
                    DestroyImmediate(debugObjects[i]);
                    debugObjects.RemoveAt(i);
                }
                i++;
            }
            else
            {
                DestroyImmediate(debugObjects[i]);
                debugObjects.RemoveAt(i);
                debugObjectsType.RemoveAt(0);
            }
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

        if(Time.time > currTimeA + avoidAreaRate)
        {
            currTimeA = Time.time;
            debugObjects.Add(Instantiate(areaAvoid, transform.position - transform.forward * 1 + new Vector3(Random.Range(-0.6f, 0.6f), 0, Random.Range(-0.6f, 0.6f)), Quaternion.identity) as GameObject);

        }



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

        Vector3 minimumVector = new Vector3(0.00001f, 0.00001f, 0.00001f);
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
        Vector3 averageAvoidPosition = (transform.position);
        List<int> indeces = new List<int>();
        int index = 0;
        foreach (edgeType thisEdge in debugObjectsType)
        {
            if (thisEdge == edgeType.convex)
                indeces.Add(index);
            index++;
        }
        foreach (int index_ in indeces)///////////////////////////////////add convex corners to decrease distance traveled
        {
            if ((debugObjects[index_].GetComponent<Transform>().position - transform.position).magnitude < 3) { }
                //averageAvoidPosition += (debugObjects[index_].GetComponent<Transform>().position - transform.position) * 12 + minimumVector;// / (debugObjects[index_].GetComponent<Transform>().position - transform.position).magnitude;
        }
        Vector3 minDistVector = new Vector3(1000,1000,1000);
        for(int i = 0; i < 8; i++){////////////////////////////////////////add radar cast vectors
            float preference = 1;// -0.08f * Mathf.Abs(i - 4) + 1;
            Vector3 localVector = (wallHits[i] * preference - transform.position);
            if(localVector.magnitude < minDistVector.magnitude){
                minDistVector = localVector;
            }
            averageAvoidPosition -= 6 * debugObjects.Count * (localVector - transform.position).normalized / Mathf.Pow(Mathf.Abs((localVector - transform.position).magnitude), 3f);// / localVector.magnitude;
        }
        foreach (GameObject avoid in debugObjects)/////////////////////////add previous locations
        {
            if (avoid.GetComponent<AvoidArea>() != null)
            {
                float spatialApathy = 2f / (1 + Mathf.Pow((avoid.transform.position - transform.position).magnitude / 3, 1.6f));
                float timeExisted = avoid.GetComponent<AvoidArea>().RemoveTimer;
                float timeAllowed = avoid.GetComponent<AvoidArea>().Remove;
                float temporalApathy = (timeExisted - Time.time) * 0.7f / timeAllowed;
                //temporalApathy = temporalApathy * -0.1f + 1;
                //averageAvoidPosition -= (avoid.transform.position - transform.position) * temporalApathy * spatialApathy * 1f;
            }
        }


        averageAvoidPosition.y = 0;//transform.position.y;
        
        debugObjects.Add(Instantiate(debugObject, averageAvoidPosition, transform.rotation) as GameObject);
        debugObj = debugObjects[debugObjects.Count - 1];
        debugObj.GetComponent<Renderer>().material.color = new Color(1,0,0);
        debugObj.transform.localScale *= 5;
        debugObjectsType.Add(edgeType.convex);
        //move with slight random headings
        charSelf.turn(40 * randAngleMult * Time.deltaTime);
        //avoid walls and concave corners
        charSelf.turn(averageAvoidPosition, 35 * Time.deltaTime / (minDistVector.magnitude / 3 ));
        
        charSelf.move(transform.forward, 2f);
    }
}
