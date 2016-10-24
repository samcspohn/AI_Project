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
    public void move(float moveRight, float moveForward, float speed)
    {
        movementVector.Set(moveRight, 0, moveForward);
        movementVector.Normalize();
        movementVector *= speed * Time.deltaTime;
        transform.Translate(movementVector);
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
