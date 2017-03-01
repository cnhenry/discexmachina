using System.Collections;
using UnityEngine;
using Valve.VR;

public class ControllerDiscHelper : MonoBehaviour {

    public GameObject discToSpawn;
    public Rigidbody discAttachPoint;
    public float recallTimeDelay = 3.0f; //Wait a specific amount of time before allowing a disc recall

    private FixedJoint attachedJoint;
    private EVRButtonId triggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_TrackedObject trackedObj;
    private float lastTimeInteracted;
    private GameObject currentDisc;
    public bool triggerButtonPressed = false;

    private SteamVR_Controller.Device controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
        //Spawn new disc on trigger press
        if ( controller.GetPressDown(triggerButton) && attachedJoint == null && Time.time - lastTimeInteracted > recallTimeDelay) {
            Debug.Log("Trigger Pressed && attachedJoint == null && recallTime satisfied");
            if ( currentDisc != null ) { //Remove disc if in play
                Debug.Log("Object in play. Deleting.");
                Object.Destroy(currentDisc);
            }
            //Create new disc
            currentDisc = GameObject.Instantiate(discToSpawn);

            //Attach the disc to the joint
            //attachedJoint = currentDisc.AddComponent<FixedJoint>();
            //attachedJoint.connectedBody = discAttachPoint;
            currentDisc.SetActive(true);
            Debug.Log("Attached spawned disc to attachpoint (joint)");          
        }

        //Continue updating the position of the object as trigger is held
        if ( controller.GetPress(triggerButton) ) {
            currentDisc.transform.position = transform.position;
            currentDisc.transform.rotation = transform.rotation;
        }

        //Release disc on trigger release
        if ( controller.GetPressUp(triggerButton) ) {
            Debug.Log("Trigger released, joint destroyed");
            Object.Destroy(attachedJoint);
            //attachedJoint = null;
        }
    }
}
