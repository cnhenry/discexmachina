// Author - Cyrus Duong
// Date - 03-30-2017

using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class DiscSpawner : NetworkBehaviour {

    public GameObject discToSpawn;
    public GameObject leftController, rightController;
    public GameObject leftControllerAttachPoint, rightControllerAttachPoint;
    public float grabableRange;

    private bool controllersGood = false;

    private SteamVR_TrackedObject leftTrackedObj;
    private SteamVR_TrackedObject rightTrackedObj;
    private SteamVR_Controller.Device Steam_LeftController;
    private SteamVR_Controller.Device Steam_RightController;
    
    void Start() {
        if ( !isLocalPlayer ) {
            return;
        }
        //Grab the steam controller objects and obtain the SteamVR_Controller to detect trigger presses
        leftTrackedObj = leftController.GetComponentInChildren<SteamVR_TrackedObject>();
        rightTrackedObj = rightController.GetComponentInChildren<SteamVR_TrackedObject>();

        Debug.Log("Left=" + (int)leftTrackedObj.index + " Right=" + (int)rightTrackedObj.index);
        if ( (int)leftTrackedObj.index >= 0 && (int)rightTrackedObj.index >= 0 ) {
            Debug.Log("Controllers Assigned");
            Steam_LeftController = SteamVR_Controller.Input((int)leftTrackedObj.index);
            Steam_RightController = SteamVR_Controller.Input((int)rightTrackedObj.index);
            controllersGood = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if ( !isLocalPlayer || !controllersGood ) { return; }

        if ( Steam_LeftController.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) ) {
            Debug.Log("Left Spawning");
            Cmd_Spawn(leftController.transform.position, leftController.transform.rotation);
        }

        if ( Steam_RightController.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) ) {
            Debug.Log("Right Spawning");
            Cmd_Spawn(rightController.transform.position, rightController.transform.rotation);
        }

        if ( Steam_LeftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            Debug.Log("Left Grabbing");
            GrabObject(leftControllerAttachPoint);
        }

        if ( Steam_LeftController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) ) {
            Debug.Log("Left Released");
            ReleaseObject(leftControllerAttachPoint, Steam_LeftController);
        }

        if ( Steam_RightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            Debug.Log("Right Grabbing");
            GrabObject(rightControllerAttachPoint);
        }

        if ( Steam_RightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) ) {
            Debug.Log("Right Released");
            ReleaseObject(rightControllerAttachPoint, Steam_RightController);
        }
    }

    //Function to grab objects and attachThem to the given attachPoint's Rigidbody
    void GrabObject(GameObject attachPoint) {
        //Get list of interactables in the environment
        InteractablePhysicalObject[] interactables = GameObject.FindObjectsOfType(typeof(InteractablePhysicalObject)) as InteractablePhysicalObject[];

        //Calculate Closest
        InteractablePhysicalObject closestInteractable = null;
        float dist = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach ( InteractablePhysicalObject ipo in interactables ) {
            Vector3 diff = ipo.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if ( curDistance < dist && curDistance < grabableRange ) {
                closestInteractable = ipo;
                dist = curDistance;
            }
        }

        //Grab the closest interactable
        if ( closestInteractable != null ) {
            Debug.Log("Found close InteractablePhysicalObjects");
            // Flag interactable as active.
            closestInteractable.beingInteracted = true; 

            // Set the position of the interactable and joint to rigidbody attachpoint on controllers
            closestInteractable.gameObject.transform.position = attachPoint.transform.position + closestInteractable.grabOffsetPosition;
            closestInteractable.gameObject.transform.rotation = attachPoint.transform.rotation * Quaternion.Euler(closestInteractable.grabOffsetRotation);

            //Add a joint onto the attachpoint
            FixedJoint joint = attachPoint.AddComponent<FixedJoint>();

            //Connect it to the interactable's rigidbody
            joint.connectedBody = closestInteractable.gameObject.GetComponent<Rigidbody>();
        }
    }

    //Function to release objects attached to the attachPoint's Rigidbody
    void ReleaseObject(GameObject attachPoint, SteamVR_Controller.Device device) {
        FixedJoint joint = attachPoint.GetComponent<FixedJoint>();

        if ( joint != null ) {
            //Retrieve the interactable from the object
            InteractablePhysicalObject releasedInteractable = joint.connectedBody.gameObject.GetComponent<InteractablePhysicalObject>();
            Debug.Log("releasedInteractable=" + releasedInteractable);
            //Destroy the joint connecting the two gameObjects 'releasing' the object
            DestroyImmediate(joint);
            joint = null;

            //Apply physics to the interactable as if it were carrying momentum if there were any from the attachpoint
            float velocityMultiplier, angularVelocityMultiplier;
            velocityMultiplier = releasedInteractable.throwMultiplier;
            angularVelocityMultiplier = releasedInteractable.angularThrowMultiplier;

            Rigidbody rb = releasedInteractable.GetComponent<Rigidbody>();
            Rigidbody attachedParentRb = attachPoint.GetComponentInParent<Rigidbody>();

            Debug.Log("attachedRb.velocity=" + attachedParentRb.velocity);

            rb.velocity = device.velocity * velocityMultiplier;
            rb.angularVelocity = device.angularVelocity * angularVelocityMultiplier;
        }
    }


    //Request Server Spawn Disc at Origin
    [Command]
    public void Cmd_Test() {
        GameObject netDisc = Instantiate(discToSpawn);
        NetworkServer.SpawnWithClientAuthority(discToSpawn, this.gameObject);
    }

    // https://docs.unity3d.com/Manual/UNetSpawning.html
    // http://answers.unity3d.com/questions/987951/unet-networkserverspawn-not-working.html
    // Command means when the client calls this function it is forwarded to the server and lets the server handle it
    [Command]
    public void Cmd_Spawn(Vector3 handPosition, Quaternion handRotation) {
        GameObject netDisc = Instantiate(discToSpawn, handPosition, handRotation);
        // https://forum.unity3d.com/threads/solved-clients-list-of-game-object-prefabs-not-shown-in-the-server-how-do-i-solve-this.352420/
        //ClientScene.RegisterPrefab(netDisc);
        //Spawn the disc on clients
        NetworkServer.Spawn(netDisc);
    }

}
