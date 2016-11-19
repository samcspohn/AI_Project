using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    public bool playerControl = false;
    public GameObject self;
    public GameObject Other;
    public GameObject debugObject;
    public GameObject areaAvoid;
    public Transform arbitraryCube;

    public int steps = 6;
    private GameObject debugObj;
    private Character Navigator;
    private NavMeshAgent agent;

    public AI ai;

    public GameObject previousNavNodeID = null;
    public GameObject currentNavNodeID;
    private GameObject nearestNode;



    public float speed = 1f;
    public float turn = 0.5f;
    //public float avoidAreaRate = 2;
    private bool reachedCover = false;
    //private bool quickStep = false;
    int avoidID = -1;

    private Gun gun;
    private Character charSelf;

    public static List<GameObject> debugObjects;
    private static List<edgeType> debugObjectsType;
    List<GameObject> cornersInView;

    private float currTime = 0;
    //private float currTimeA = 0;
    private float height;
    private float moveRight;
    private float moveForward;
    private enum edgeType{convex, concave, avoid};
    private char action = ' ';
    private float randAngleMult;

    //char action;
    int avoid;
    int avoidMask;
    bool fireOnce = true;


    void Start () {
        
        ai = transform.GetComponent<AI>();

        debugObjects = new List<GameObject>();
        debugObjectsType = new List<edgeType>();
        gun = transform.GetComponentInChildren<Gun>();
        charSelf = transform.GetComponent<Character>();
       // Navigator = transform.FindChild("Navigator").gameObject.transform.GetComponent<Character>();
        agent = transform.GetComponent<NavMeshAgent>();
       // Navigator.transform.parent = null;
        charSelf.healthPoints = 100;
        height = transform.localScale.y;

        GameObject[] allNavNodes = GameObject.FindGameObjectsWithTag("navNode");
        nearestNode = allNavNodes[0];
        foreach(GameObject navNode in allNavNodes){
        if((nearestNode.transform.position - transform.position).magnitude > (navNode.transform.position - transform.position).magnitude)
            nearestNode = navNode;
        }
        currentNavNodeID = nearestNode;
        agent.SetDestination(nearestNode.transform.position);

        avoid = 1 << 4;
        avoidMask = ~0 & avoid;
    }

    void Update()
    {

        //////////////////////////////////////////////////regen
        //charSelf.healthPoints += 6 * Time.deltaTime;
        for(int i = 0;  i < debugObjects.Count;){
            if(debugObjects[i] != null && debugObjects[i].GetComponent<AvoidArea>() != null)
            {
                if(debugObjects[i].GetComponent<AvoidArea>().RemoveTimer < Time.time)
                {
                    DestroyImmediate(debugObjects[i]);
                    debugObjects.RemoveAt(i);
                    debugObjectsType.RemoveAt(i);
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

        bool playerInSight = false;
        float playerAngle = 0f;
        int numOfRayHits = 0;
        int targetHeight = 0;
        //closestEdge = arbitraryCube;


        GameObject[] debugObjectsSimple = GameObject.FindGameObjectsWithTag("convex");
        cornersInView = new List<GameObject>();
        foreach(GameObject convexCorner in debugObjectsSimple){
            RaycastHit hit;
            Vector3 rayDirection = convexCorner.transform.position - transform.position;
            if((Vector3.Angle(rayDirection, transform.forward)) <= 120 * 0.5f){ // Detect if player is within the field of view
                if (Physics.Raycast (transform.position, rayDirection, out hit)) {
                    if (hit.transform.tag == "convex") {
                        //Debug.Log("Can see corner");
                        cornersInView.Add(convexCorner);
                    }
                }
            }
        }

        // if (playerInSight){
        //     if(targetHeight == 1)
        //     {
        //         charSelf.crouch();
        //     }
        //     else if(targetHeight == 2)
        //     {
        //         charSelf.uncrouch();
        //     }
        //     playerAngle = playerAngle / numOfRayHits;
        //     transform.GetComponent<Renderer>().material.color = Color.green;
        //     transform.Rotate(0, playerAngle, 0);
        //     gun.fire();
        // }else{
        //     charSelf.uncrouch();
        //     transform.GetComponent<Renderer>().material.color = Color.red;
        // }
        //----------------------------------------------debug enemy move
        if(playerControl){
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
            charSelf.move(moveRight,moveForward,speed);
        }
        //////////////////////////////////////////////////////////////////////////////automatic movement

        ///////////////////////////////////////////input
        if(Other != null){

            if(Time.time > currTime + turn){
                action = ai.think(steps);
                currTime = Time.time;
                fireOnce = true;
            }
            
            ////////////////////////////////////////////output
            
            if (Time.time < currTime + turn)// move to edge in a turn
            {
            // agent.Resume();
                GameObject closestCorner = getNearestCorner();
                if(action == 'p' && fireOnce){// && closestCorner != null)
                Debug.Log("taking cover");
                    takeCover();
                }
                else if(EisVisible()){
                    Debug.Log("shooting");
                    shoot();
                }
                else if(action == 'o' && fireOnce){
                    Debug.Log("patrolling");
                    patrol();
                    //quickStep = false;
                }
                fireOnce = false;

            }else{
                //agent.Stop();
            }
        }

        //----------------------------------------------end of enemy move
        // Navigator.move(moveRight, moveForward, speed);
//        Navigator.transform.position = new Vector3(Navigator.transform.position.x, 0.8f, Navigator.transform.position.z);
  //      transform.position = Navigator.transform.position;
        // hide behind cover
        if (charSelf.healthPoints <= 0)
        {
            Destroy(self);
        }
        transform.rotation.eulerAngles.Set(0, transform.rotation.eulerAngles.y, 0);
    }
    public bool EisVisible(){
		RaycastHit hit;
		if(Physics.Raycast(self.transform.position, Other.transform.position - self.transform.position, out hit)){
			if(hit.collider.gameObject == Other){
				return true;
			}else{
				return false;
			}
		}
		return false;
	}
    void patrol_(float randAngleMult, bool quickStep){

        Vector3 minimumVector = new Vector3(0.00001f, 0.00001f, 0.00001f);
        Vector3 averageAvoidPosition = (transform.position);

        foreach(GameObject convexCorner in cornersInView){ //////////////////////////////////////////add convex corners to decrease distance traveled
            averageAvoidPosition += (convexCorner.transform.position - transform.position) * Mathf.Clamp((convexCorner.transform.position - transform.position).magnitude, 0.02f, 1) * 4;
        }

        foreach (GameObject avoid in debugObjects) /////////////////////////////////////////////////////subtract previous locations**************also important
        {
            if (avoid.GetComponent<AvoidArea>() != null)
            {
                float spatialApathy = 2f / (1 + Mathf.Pow((avoid.transform.position - transform.position).magnitude / 2, 1.8f));
                float timeExisted = avoid.GetComponent<AvoidArea>().RemoveTimer;
                float timeAllowed = avoid.GetComponent<AvoidArea>().Remove;
                float temporalApathy = 1 / (1 + Mathf.Pow((timeExisted - Time.time), 2));
                averageAvoidPosition -= (avoid.transform.position - transform.position) * temporalApathy * spatialApathy;
            }
        }
        averageAvoidPosition.Normalize();
        averageAvoidPosition *= 0.5f;
        Vector3 Obstacles = avoidObstacles();
        Obstacles.Normalize();
        Obstacles *= 0.5f;
        averageAvoidPosition += avoidObstacles(); // steer clear of walls
        averageAvoidPosition.y = 1f;
        
        debugObjects.Add(Instantiate(debugObject, averageAvoidPosition, transform.rotation) as GameObject);//add debug anti-av avoid object
        debugObj = debugObjects[debugObjects.Count - 1];
        debugObj.GetComponent<Renderer>().material.color = new Color(1,0,0);
        debugObj.transform.localScale *= 5;
        debugObjectsType.Add(edgeType.concave);
        
        charSelf.turn(120 * randAngleMult * Time.deltaTime / (quickStep ? Time.deltaTime / speed : 1)); //move with slight random headings
        
        Navigator.turn(averageAvoidPosition, 260 * Time.deltaTime / (quickStep ? Time.deltaTime / 2 : 1)); // avoid previous areas
        //charSelf.move(averageAvoidPosition, 0.01f);
        RaycastHit futurePos;
        if(Physics.Raycast(Navigator.transform.position, Navigator.transform.forward, out futurePos)){
            if((futurePos.point - transform.position).magnitude < Navigator.transform.forward.magnitude * speed / 2.3 && quickStep){
                Navigator.transform.Translate((futurePos.point - transform.position) * 0.5f);
            }
            else{
                Navigator.move(Navigator.transform.forward, speed / (quickStep ? Time.deltaTime / speed : 1));
                Debug.Log("quick step");
            }
        }
        Navigator.transform.position = new Vector3(Navigator.transform.position.x, 0.8f, Navigator.transform.position.z);
        transform.position = Navigator.transform.position;
    }
    public void patrol(){
        NavNode currNode = currentNavNodeID.GetComponent<NavNode>(); 
        if(currNode.forward != null && currNode.forward != previousNavNodeID){
            agent.SetDestination(currNode.forward.transform.position);
           // Debug.Log("going forward");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.forward;
        } 
        else if(currNode.left != null && currNode.left != previousNavNodeID){
            agent.SetDestination(currNode.left.transform.position);
           // Debug.Log("going left");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.left;
        } 
        else if(currNode.right != null && currNode.right != previousNavNodeID){
            agent.SetDestination(currNode.right.transform.position);
            //Debug.Log("going right");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.right;
        } 
        else if(currNode.backward != null && currNode.backward != previousNavNodeID){
            agent.SetDestination(currNode.backward.transform.position);
            //Debug.Log("going backward");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.backward;
        }

    }

    GameObject getNearestCorner(){
        GameObject[] debugObjectsSimple = GameObject.FindGameObjectsWithTag("convex");
        GameObject closestCorner = null;// = debugObjectsSimple[0];
        foreach(GameObject convexCorner in debugObjectsSimple){
            RaycastHit hit;
            Vector3 rayDirection = convexCorner.transform.position - transform.position;
            if (Physics.Raycast (transform.position, rayDirection, out hit)) {
                if (hit.transform.tag == "convex"){
                    closestCorner = (closestCorner == null ? convexCorner : ((closestCorner.transform.position - transform.position).magnitude > (convexCorner.transform.position - transform.position).magnitude) ? convexCorner : closestCorner);
                }
            }
        }
        return closestCorner;
    }

    void takeCover_(){
        GameObject hideSpot = getNearestCorner();
        Vector3 avoidArea = avoidObstacles();
        Vector3 hidePoint = (hideSpot.transform.position + hideSpot.transform.forward * 2f);// - Navigator.transform.position);
        if((hidePoint - transform.position).magnitude < 0.6f){
            reachedCover = true;
            Debug.Log("reached cover");
        }
        if(!reachedCover){
            Navigator.move(hidePoint - transform.position, speed);// + avoidArea * 0.01f, speed); // move Navigator
            transform.position = Navigator.transform.position;// put the enemy at the Navigators position
        }
        
        if(reachedCover){
            GameObject other =  GameObject.FindGameObjectWithTag("Player");
            Vector3 rayDirection = hideSpot.transform.position - other.transform.position;
            RaycastHit hit;
            //if(Physics.Raycast(hidePoint, rayDirection, out hit, 10000, (1 << 9))){
                if((Vector3.Angle(rayDirection, hideSpot.transform.forward)) <= 180 && (Vector3.Angle(rayDirection, hideSpot.transform.forward)) > 0){
                    Debug.Log("player is on right side of cover");
                    //move left of cover
                    Vector3 RightCover = hideSpot.transform.FindChild("leftCover").transform.position;
                    Debug.Log(RightCover);
                    Navigator.move(hideSpot.transform.FindChild("rightCover").transform.position - transform.position, speed);
                    // Navigator.turn(hideSpot.transform.FindChild("rightCover").transform.position, 10);
                    // Navigator.move(Navigator.transform.forward, speed);
                    transform.position = Navigator.transform.position;

                    if (Physics.Raycast (transform.position, rayDirection, out hit)) {
                        if (hit.transform.tag == "Player") {
                        }
                    }
                }else{//} if((Vector3.Angle(rayDirection, hideSpot.transform.forward)) <= -180 && (Vector3.Angle(rayDirection, transform.forward)) < 0){
                    Debug.Log("player is on left side of cover");
                    //move right
                    Vector3 RightCover = hideSpot.transform.FindChild("rightCover").transform.position;
                    Debug.Log(RightCover);
                    Navigator.move(hideSpot.transform.FindChild("rightCover").transform.position - transform.position, speed);
                    // Navigator.turn(hideSpot.transform.FindChild("rightCover").transform.position, 10);
                    // Navigator.move(Navigator.transform.forward, speed);
                    transform.position = Navigator.transform.position;
                }
            //}
        }
    }
    public void takeCover(){
        // GameObject hideSpot = getNearestCorner();
         GameObject other = Other;
        
        // Vector3 rayDirection = hideSpot.transform.position - other.transform.position;
        RaycastHit hit;
        //if(Physics.Raycast(hideSpot.transform.position, rayDirection, out hit, 10000, (1 << 9))){

        GameObject[] debugObjectsSimple = GameObject.FindGameObjectsWithTag("convex");
        Vector3 closestCorner = new Vector3();// = debugObjectsSimple[0];
        bool noActivation = true;
        foreach(GameObject convexCorner in debugObjectsSimple){
            Vector3 destination = convexCorner.transform.FindChild("leftCover").transform.position;
            if(convexCorner.transform.GetInstanceID() != avoidID){
                
                if(Physics.Raycast(destination, other.transform.position - destination, out hit, 1000, (1<<10))){
                    //if(hit.collider.gameObject == Other)
                    //Debug.Log("hit the player from the left side");
                    
                    //Debug.Log("this is the one");
                    if (hit.collider.gameObject == Other && (closestCorner - transform.position).magnitude > (destination - transform.position).magnitude){// && (!(Mathf.Abs(Vector3.Angle(agent.steeringTarget, other.transform.position -transform.position)) < 60) || !noActivation)){
                        //Debug.Log("did not hit the player from the left side");
                        closestCorner = destination;
                        noActivation = false;
                        agent.SetDestination(closestCorner);
                    }
                }
                destination = convexCorner.transform.FindChild("rightCover").transform.position;
                if(Physics.Raycast(destination, other.transform.position - destination, out hit, 1000, (1<<10))){
                    //if(hit.collider.gameObject == Other)
                    //Debug.Log("hit the player from the right side");
                    if (hit.collider.gameObject == Other && (closestCorner - transform.position).magnitude > (destination - transform.position).magnitude){// && (!(Mathf.Abs(Vector3.Angle(agent.steeringTarget, other.transform.position -transform.position )) < 60) || !noActivation)){
                        //Debug.Log("did not hit the player from the right side");
                        closestCorner = destination;
                        noActivation = false;
                        agent.SetDestination(closestCorner);
                    }
                }
            }else{
                //Debug.Log("this is the one");
            }
        }
        Debug.Log(noActivation);
        if(noActivation){
            Collider[] covers = Physics.OverlapSphere(agent.destination + new Vector3(0,0f,0), 0.1f);
            foreach(Collider cover in covers){
                //if(cover.gameObject.transform.parent != null){

                if(cover.gameObject.name == "leftCover" || cover.gameObject.name == "rightCover"){
                    //Debug.Log("one of the covers");
                GameObject culprit = cover.gameObject;
                //culprit.GetComponent<Renderer>().material.color = new Color(0.5f,0.6f,0.5f);
                avoidID = cover.gameObject.transform.parent.gameObject.transform.GetInstanceID();
                // }
                }
            }
        }
        if(agent.remainingDistance < 0.4f){
            GameObject[] allNavNodes = GameObject.FindGameObjectsWithTag("navNode");//reset current navNode
            GameObject closestNode = allNavNodes[0];
            foreach(GameObject navNode in allNavNodes){
            if((closestNode.transform.position - transform.position).magnitude > (navNode.transform.position - transform.position).magnitude)
                closestNode = navNode;
            }
            currentNavNodeID = closestNode;
            previousNavNodeID = null;
        }

    }
    public void shoot(){
        transform.GetChild(1).LookAt(Other.transform.position);
        Debug.Log("firing gun");
        gun.fire();
    }
    Vector3 avoidObstacles(){
        Vector3 averageAvoidPosition = (transform.position);
        Vector3 minimumVector = new Vector3(0.00001f, 0.00001f, 0.00001f);
        RaycastHit ray;
        float[] wallDists = {0,0,0};
        Vector3[] wallHits = {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
        for(int angle = 0; angle < 8; angle++){
            if(Physics.Raycast(transform.position, Quaternion.Euler(0,(angle - 4) * 45,0) * Navigator.transform.forward, out ray)){
                wallHits[angle] = ray.point;
            }
        }
        Vector3 minDistVector = new Vector3(1000,1000,1000);
        for(int i = 0; i < 8; i++){ ////////////////////////////////////////////////////////////////subtract radar cast vectors*******************important
            Vector3 localVector = (wallHits[i] - transform.position) ;
            if(localVector.magnitude < minDistVector.magnitude){
                minDistVector = localVector;
            }
            averageAvoidPosition -= (localVector) / Mathf.Pow((localVector).magnitude / 2f, 2f) * 16;// / localVector.magnitude;  
        }
        //charSelf.turn(averageAvoidPosition, 45 * Time.deltaTime / Mathf.Clamp(minDistVector.magnitude / 3, 0.001f, 2)); //avoid walls and concave corners
        return averageAvoidPosition;
    }
}


