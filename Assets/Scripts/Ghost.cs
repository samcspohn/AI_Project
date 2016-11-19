using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
	public GameObject otherGhost;
	private int avoidID;
	public GameObject self;
	public float hp = 100;
	public GameObject currentNavNodeID;
	public GameObject previousNavNodeID;


	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void takeCover(){
        // GameObject hideSpot = getNearestCorner();
         GameObject other = otherGhost;
        
        // Vector3 rayDirection = hideSpot.transform.position - other.transform.position;
        RaycastHit hit;
        //if(Physics.Raycast(hideSpot.transform.position, rayDirection, out hit, 10000, (1 << 9))){

        GameObject[] debugObjectsSimple = GameObject.FindGameObjectsWithTag("convex");
        Vector3 closestCorner = new Vector3();// = debugObjectsSimple[0];
        bool noActivation = true;
        foreach(GameObject convexCorner in debugObjectsSimple){
            Vector3 destination = convexCorner.transform.FindChild("leftCover").transform.position;
            if(convexCorner.transform.GetInstanceID() != avoidID){
                
                if(Physics.Raycast(destination, other.transform.position - destination, out hit, 1000)){
                    if(hit.collider.gameObject == otherGhost)
                    if (hit.collider.gameObject == otherGhost && (closestCorner - transform.position).magnitude > (destination - transform.position).magnitude){// && (!(Mathf.Abs(Vector3.Angle(agent.steeringTarget, other.transform.position -transform.position)) < 60) || !noActivation)){
                        closestCorner = destination;
                        noActivation = false;
                    }
                }
                destination = convexCorner.transform.FindChild("rightCover").transform.position;
                if(Physics.Raycast(destination, other.transform.position - destination, out hit, 1000)){
                    if(hit.collider.gameObject == otherGhost)
                    if (hit.collider.gameObject == otherGhost && (closestCorner - transform.position).magnitude > (destination - transform.position).magnitude){// && (!(Mathf.Abs(Vector3.Angle(agent.steeringTarget, other.transform.position -transform.position )) < 60) || !noActivation)){
                        closestCorner = destination;
                        noActivation = false;
                    }
                }
            }else{
            }
        }
        //Debug.Log(noActivation);
        // if(noActivation){
        //     Collider[] covers = Physics.OverlapSphere(agent.destination + new Vector3(0,0f,0), 0.1f);
        //     foreach(Collider cover in covers){
        //         //if(cover.gameObject.transform.parent != null){

        //         if(cover.gameObject.name == "leftCover" || cover.gameObject.name == "rightCover"){
        //             Debug.Log("one of the covers");
        //         GameObject culprit = cover.gameObject;
        //         //culprit.GetComponent<Renderer>().material.color = new Color(0.5f,0.6f,0.5f);
        //         avoidID = cover.gameObject.transform.parent.gameObject.transform.GetInstanceID();
        //         // }
        //         }
        //     }
        // }
		// transform.position = closestCorner;
        // if(agent.remainingDistance < 0.4f){
        //     GameObject[] allNavNodes = GameObject.FindGameObjectsWithTag("navNode");//reset current navNode
        //     GameObject closestNode = allNavNodes[0];
        //     foreach(GameObject navNode in allNavNodes){
        //     if((closestNode.transform.position - transform.position).magnitude > (navNode.transform.position - transform.position).magnitude)
        //         closestNode = navNode;
        //     }
        //     currentNavNodeID = closestNode;
        //     previousNavNodeID = null;
        // }

    }
	public void patrol(){
        NavNode currNode = currentNavNodeID.GetComponent<NavNode>(); 
        if(currNode.forward != null && currNode.forward != previousNavNodeID){
            transform.position = currNode.forward.transform.position;
            //Debug.Log("going forward");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.forward;
        } 
        else if(currNode.left != null && currNode.left != previousNavNodeID){
            transform.position = currNode.left.transform.position;
            ///Debug.Log("going left");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.left;
        } 
        else if(currNode.right != null && currNode.right != previousNavNodeID){
            transform.position = currNode.right.transform.position;
            //Debug.Log("going right");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.right;
        } 
        else if(currNode.backward != null && currNode.backward != previousNavNodeID){
            transform.position = currNode.backward.transform.position;
            //Debug.Log("going backward");
            previousNavNodeID = currentNavNodeID;
            currentNavNodeID = currNode.backward;
        }

    }
	public void shoot(){
        Vector3 lookAtVector = new Vector3(otherGhost.transform.position.x, transform.position.y, otherGhost.transform.position.z);
        transform.LookAt(otherGhost.transform.position);
		otherGhost.transform.GetComponent<Ghost>().Damage();
    }
	public void Damage(){
		hp -= 15;
	}
}
