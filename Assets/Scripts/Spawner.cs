using UnityEngine;
using System.Collections;

public class Spawner : InteractableTriggerObject
{
    public GameObject prefab;
    public Rigidbody spawnLocation;
    public float delayInSeconds = 1.0f;

    float lastTime;
    FixedJoint joint;

    // Use this for initialization
    void Start()
    {
        lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (joint != null)
        {
            var go = joint.gameObject;
            InteractableObject io = go.GetComponent<InteractableObject>();

            if (io != null)
            {
                if (io.beingInteracted == true)
                {
                    Object.DestroyImmediate(joint);
                    joint = null;
                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastTime > delayInSeconds && joint == null)
        {
            var go = GameObject.Instantiate(prefab);
            go.transform.position = spawnLocation.transform.position;
            Debug.Log(go.transform.position);
            lastTime = Time.time;

            joint = go.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = spawnLocation;
        }
    }
}
