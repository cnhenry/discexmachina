using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class InteractablePhysicalObject : InteractableObject {

    public Vector3 grabOffsetPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 grabOffsetRotation = new Vector3(0.0f, 0.0f, 0.0f);
    public float throwMultiplier = 1.0f;
    public float angularThrowMultiplier = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
