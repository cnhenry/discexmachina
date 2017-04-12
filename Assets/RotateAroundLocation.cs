using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundLocation : MonoBehaviour {
    public Vector3 point;
    public Vector3 axis;
    public float speed;
    // Update is called once per frame
    void Update() {
        transform.RotateAround(point, axis, speed * Time.deltaTime);
    }
}
