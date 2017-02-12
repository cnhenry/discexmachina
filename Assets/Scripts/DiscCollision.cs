using UnityEngine;
using System.Collections;

public class DiscCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter (Collision col) {
        //Debug.Log("I have collided with: " + col.gameObject.name + " and the tags are: " + col.gameObject.tag);
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 forcevector = -5 * col.relativeVelocity;

        if (col.gameObject.CompareTag("Wall"))
        {
            rigidbody.AddForce(forcevector, ForceMode.Acceleration);
        }
    }

}
