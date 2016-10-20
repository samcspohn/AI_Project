using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    private float healthPoints = 1;
    float playerSpeed = 2;
    float playerRotation = 65;
    private Gun gun;
    private Character charSelf;

	// Use this for initialization
	void Start () {
        healthPoints = 100f;
        gun = transform.GetComponentInChildren<Gun>();
        charSelf = transform.GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.forward, out hit)) {
			//print ("Found an object - distance: " + hit.distance);
			Debug.DrawLine (transform.position, hit.point, Color.green);
			//print(hit.collider.tag);
		}

        var moveRight = Input.GetAxisRaw("Horizontal");
        var moveForward = Input.GetAxisRaw("Vertical");
        var camRotateHorizontal = Input.GetAxisRaw("Mouse X") * Time.deltaTime * playerRotation;
        var crouch = Input.GetKeyDown("space");
        var unCrouch = Input.GetKeyUp("space");

        charSelf.turn(camRotateHorizontal);
        charSelf.move(moveRight, moveForward, playerSpeed);
        if (crouch)
        {
            charSelf.crouch();
        }
        else if(unCrouch)
        {
            charSelf.uncrouch();
        }
        if (Input.GetMouseButtonDown(0))
        {
            gun.fire();
        }

	}
}
