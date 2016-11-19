using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    public float healthPoints;
    private Vector3 movementVector;

    public void turn(float rotation)
    {
        transform.Rotate(Vector3.up * rotation);
    }

    public void turn(Vector3 turnTowards, float angularSpeed)
    {   
        Vector3 localTurnTowards = transform.InverseTransformDirection(turnTowards - transform.position);
        Vector3 newDir =  localTurnTowards.normalized;
        if(newDir.x < 0){
            angularSpeed = -angularSpeed;
        }
        float angle = ((newDir.z < 0 ? Mathf.PI : 0) + Mathf.Atan(newDir.z / newDir.x)) / Mathf.PI;
        transform.Rotate(0,angularSpeed * (angle * angle + 0.1f), 0);
    }
    public void move(float moveRight, float moveForward, float speed)
    {
        if(moveRight != 0 || moveForward != 0){
            movementVector.Set(moveRight, 0, moveForward);
            movementVector.Normalize();
            movementVector *= speed * Time.deltaTime;
            transform.Translate(movementVector);    
        }
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
