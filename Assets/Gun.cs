using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
    public Transform gun;
    public GameObject bullet;
    private int magSize = 6;
    private int bulletsInMag;
    private float refireRate = 0.5f;
    private float reloadTime = 1.5f;
    private float nextFire = 0f;
    // Use this for initialization
    void Start () {
        bulletsInMag = magSize;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    public void fire()
    {
        if(Time.time > nextFire){
            bulletsInMag--;
            GameObject clone = Instantiate(bullet, transform.position, transform.rotation) as GameObject;
            clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * 1050);
            nextFire = Time.time;
            if (bulletsInMag == 0)
            {
                nextFire += reloadTime;
                bulletsInMag = magSize;
            }else{
                nextFire += refireRate;
            }
            
        }
    } 
}
