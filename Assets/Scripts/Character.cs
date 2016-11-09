using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    public float healthPoints;
    private Vector3 movementVector;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void turn(float rotation)
    {
        transform.Rotate(Vector3.up * rotation);
    }
    public void turn_(Vector3 turnTowards, float angularSpeed)
    {
        //float step = angularSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, turnTowards, angularSpeed, 0.0F);
        //Debug.DrawRay(transform.position, newDir, Color.red);
        transform.rotation = Quaternion.LookRotation(newDir);
        //transform.Rotate(Vector3.up * rotation);
    }

    public void turn(Vector3 turnTowards, float angularSpeed)
    {   
        Vector3 localTurnTowards = transform.InverseTransformDirection(turnTowards - transform.position);
        Vector3 newDir =  localTurnTowards.normalized;// - transform.forward;// - turnTowards.normalized;
        //float degrees = Mathf.Tan(Mathf.Abs(newDir.x / newDir.z));
        if(newDir.x < 0){// || newDir.z < 0){
            //degrees = -degrees;
            angularSpeed = -angularSpeed;
        }
        transform.Rotate(0,angularSpeed, 0);
        //float step = angularSpeed * Time.deltaTime;
        //Vector3 newDir = Vector3.RotateTowards(transform.forward, turnTowards, angularSpeed, 0.0F);
        //Debug.DrawRay(transform.position, newDir, Color.red);
        //transform.rotation = Quaternion.LookRotation(newDir);
        //transform.Rotate(Vector3.up * rotation);
    }
    public void move(float moveRight, float moveForward, float speed)
    {
        movementVector.Set(moveRight, 0, moveForward);
        movementVector.Normalize();
        movementVector *= speed * Time.deltaTime;
        transform.Translate(movementVector);
    }
    public void move(Vector3 direction, float speed)
    {
        movementVector = direction;
        movementVector.Normalize();
        movementVector *= speed * Time.deltaTime;
        transform.Translate(movementVector, Space.World);
    }
    public void crouch()
    {
        //Debug.Log("crouch pressed");
        transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
        //transform.Translate(0, -0.1f, 0);
    }
    public void uncrouch()
    {
        transform.localScale = new Vector3(0.35f, 0.45f, 0.35f);
        //transform.Translate(0, 0.1f, 0);
    }
    public void TakeDamage(float damage)
    {
        healthPoints -= damage;
    }
}
