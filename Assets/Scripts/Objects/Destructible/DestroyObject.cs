using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour {
    public float timeToDestroy;
	// Use this for initialization
	void Start () {
        // Destroys GameObject after x seconds
        Destroy(gameObject, timeToDestroy);
	}
}
