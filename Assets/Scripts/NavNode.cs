using UnityEngine;
using System.Collections;

public class NavNode : MonoBehaviour {


	public GameObject forward;
	public GameObject left;
	public GameObject right;
	public GameObject backward;


	public GameObject player1;
	public GameObject player2;
	public float recentVisit1 = 0;
	public float recentVisit2 = 0;
	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Enemy");
		player2 = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

	}
}
