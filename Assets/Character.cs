using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
