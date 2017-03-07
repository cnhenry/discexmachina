using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoTracker : MonoBehaviour {
    public GameObject head;
    public Vector3 headOffset = new Vector3(0.0f, -0.05f, 0.0f);
    public Vector3 torsoRotationOffset = new Vector3(0.0f, 90.0f, 0.0f);
	// Use this for initialization
	void Start () {
		if (head == null)
        {
            
        }
    }

    // Update is called on a fixed cycle
    void FixedUpdate () {
        Transform headTransform = head.GetComponent<Transform>();
        Vector3 target_vector = headTransform.right;
        target_vector.y = transform.position.y;
        //Debug.Log("target_vector: " + target_vector);
        //transform.LookAt(target_vector); // for some reason doesn't work!
        transform.eulerAngles = new Vector3(0.0f, headTransform.eulerAngles.y, 0.0f) + torsoRotationOffset;
        transform.position = headTransform.position + headOffset;
    }
}
