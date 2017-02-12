using UnityEngine;
using System.Collections;

public class NewDiscTest : InteractablePhysicalObject {
    public Rigidbody rb;
    private bool hitWall;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        hitWall = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if (beingInteracted == false)
        {
            if (hitWall)
            {

            } else
            {
                rb.AddForce(new Vector3(1f * Time.deltaTime, 0f * Time.deltaTime, 0f * Time.deltaTime), ForceMode.VelocityChange);
            }
        } else
        {

        }
	}

    void FixedUpdate()
    {
        if (beingInteracted == false)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
        }
    }

    void onCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.tag == "Wall")
        {
            hitWall = true;
        }
    } 
}
