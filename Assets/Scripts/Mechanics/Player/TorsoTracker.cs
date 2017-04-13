using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoTracker : MonoBehaviour {
    public GameObject head, neck;
    public Vector3 headOffset = new Vector3(0.0f, -0.05f, 0.0f);
    public Vector3 torsoRotationOffset = new Vector3(0.0f, 90.0f, 0.0f);
	// Use this for initialization
	void Start () {
    }

    // Update is called on a fixed cycle
    void FixedUpdate () {
        Transform neckTransform = neck.GetComponent<Transform>();
        Transform headTransform = head.GetComponent<Transform>();

        transform.eulerAngles = new Vector3(0.0f, headTransform.eulerAngles.y, 0.0f) + torsoRotationOffset;
        transform.position = neckTransform.position + headOffset;
    }
}
