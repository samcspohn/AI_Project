using UnityEngine;
using System.Collections;

public class AvoidArea : MonoBehaviour {

    public float RemoveTimer = 15;
    public float Remove;
	// Use this for initialization
	void Start () {
        Remove = RemoveTimer;
        RemoveTimer += Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
