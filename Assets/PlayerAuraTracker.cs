using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuraTracker : MonoBehaviour {
    public GameObject TrackXZAxis;

	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(TrackXZAxis.transform.position.x, transform.parent.position.y + 0.1f, TrackXZAxis.transform.position.z);
	}
}