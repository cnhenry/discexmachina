using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class ControllerDiscSpawner: NetworkBehaviour {

    public GameObject discToSpawn;
    public Rigidbody attachPoint;
    public float recallTimeDelay = 3.0f; //Wait a specific amount of time before allowing a disc recall
    public float velocityMultiplier = 1.0f;
    public float angularVelocityMultiplier = 1.0f;

    private float grabbableRange = 0.05f;
    private EVRButtonId triggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_TrackedObject trackedObj;
    private float lastTimeReleased;
    private GameObject currentDisc;
    private FixedJoint joint;

    private SteamVR_Controller.Device controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        ClientScene.RegisterPrefab(discToSpawn);
    }

    // https://docs.unity3d.com/Manual/UNetSpawning.html
    public void Spawn() {
        //Create new disc on controllers 
        GameObject netDisc = (GameObject)Instantiate(discToSpawn, this.transform.position, this.transform.rotation);

        //Spawn the disc network sided
        if ( isLocalPlayer ) {
            NetworkServer.Spawn(netDisc);
        } else {
            //Todo: Fix this
            //NetworkServer.SpawnWithClientAuthority(netDisc, base.connectionToClient);
        }
        currentDisc = netDisc;
    }
	
	// Update is called once per frame
	void Update () {
        //Spawn new disc on trigger press
        if ( controller.GetPressDown(triggerButton) && Time.time - lastTimeReleased > recallTimeDelay) {
            Debug.Log("Trigger Pressed && attachedJoint == null && recallTime satisfied");
            if ( currentDisc != null ) { //Remove disc if in play
                Debug.Log("Disc for this controller in play. Deleting.");
                Object.Destroy(currentDisc);
                Object.DestroyImmediate(joint);
            }

            //Network Spawn a disc
            Spawn();

            //Attach the disc to the attachpoint with a joint
            joint = currentDisc.AddComponent<FixedJoint>();
            joint.connectedBody = attachPoint;
        }

        //While trigger button is released detach the disc and send it flying!
        if ( controller.GetPressUp(triggerButton) ) {
            //Retrieve the rigidbody of the disc
            Rigidbody rbDisc = currentDisc.GetComponent<Rigidbody>();

            //Remove the joint from the disc and attachpoint
            Object.DestroyImmediate(joint);
            joint = null;

            // ------------ ???? -----------
            // We should probably apply the offset between trackedObj.transform.position
            // and device.transform.pos to insert into the physics sim at the correct
            // location, however, we would then want to predict ahead the visual representation
            // by the same amount we are predicting our render poses. 

            // (Alvin was this you???)
            // Not sure what this does but it works...
            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if ( origin != null ) {
                rbDisc.velocity = origin.TransformVector(controller.velocity) * velocityMultiplier;
                //rbDisc.angularVelocity = origin.TransformVector(controller.angularVelocity) * angularVelocityMultiplier;
            } else {
                rbDisc.velocity = controller.velocity * velocityMultiplier;
                //rbDisc.angularVelocity = controller.angularVelocity * angularVelocityMultiplier;
            }

            //rbDisc.maxAngularVelocity = rbDisc.angularVelocity.magnitude;
            // ------------ ???? -----------
        }
    }
}
