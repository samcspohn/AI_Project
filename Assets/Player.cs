using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    private float healthPoints = 1;
    float playerSpeed = 2;
    float playerRotation = 100;
    public Transform gun;
    public GameObject bullet;
	// Use this for initialization
	void Start () {
        healthPoints = 100f;
	}
	
	// Update is called once per frame
	void Update () {
        var moveRight = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        var moveForward = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        var camRotateHorizontal = Input.GetAxis("Mouse X") * Mathf.Pow(Time.deltaTime, 0.8f) * playerRotation;
        var crouch = Input.GetKeyDown("space");
        var unCrouch = Input.GetKeyUp("space");

        transform.Rotate(Vector3.up * camRotateHorizontal);
        transform.Translate(moveRight, 0, moveForward);
        if (crouch)
        {
            Debug.Log("crouch pressed");
            transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
            transform.Translate(0, -0.1f, 0);
        }
        else if(unCrouch)
        {
            transform.localScale = new Vector3(0.35f, 0.45f, 0.35f);
            transform.Translate(0, 0.1f, 0);
        }
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clone = Instantiate(bullet, gun.position, gun.rotation) as GameObject;
            clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * 350);
        }

	}
}
