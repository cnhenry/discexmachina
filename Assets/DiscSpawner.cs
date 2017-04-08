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
    public float resetTime;

    private float elapsedTime;
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
            //Debug.Log("Controllers Assigned");
            Steam_LeftController = SteamVR_Controller.Input((int)leftTrackedObj.index);
            Steam_RightController = SteamVR_Controller.Input((int)rightTrackedObj.index);
            controllersGood = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if ( !isLocalPlayer || !controllersGood ) { return; }

        if ( Steam_LeftController.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) ) {
            //Debug.Log("Left Spawning");
            //Cmd_Spawn(leftController.transform.position, leftController.transform.rotation);
        }

        if ( Steam_RightController.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) ) {
            //Debug.Log("Right Spawning");
            //Cmd_Spawn(rightController.transform.position, rightController.transform.rotation);
        }

        if ( Steam_LeftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Left Grabbing");
            Cmd_Spawn(leftController.transform.position, leftController.transform.rotation);
            Cmd_Grab(true);
        }

        if ( Steam_LeftController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Left Released");
            Cmd_Release(true, Steam_LeftController.velocity, Steam_LeftController.angularVelocity);
        }

        if ( Steam_RightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Right Grabbing");
            Cmd_Spawn(rightController.transform.position, rightController.transform.rotation);
            Cmd_Grab(false);
        }

        if ( Steam_RightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Right Released");
            Cmd_Release(false, Steam_RightController.velocity, Steam_RightController.angularVelocity);
        }
    }

    // https://docs.unity3d.com/Manual/UNetSpawning.html
    // http://answers.unity3d.com/questions/987951/unet-networkserverspawn-not-working.html
    [Command]
    private void Cmd_Spawn(Vector3 handPosition, Quaternion handRotation) {
        GameObject netDisc = Instantiate(discToSpawn, handPosition, handRotation);
        // https://forum.unity3d.com/threads/solved-clients-list-of-game-object-prefabs-not-shown-in-the-server-how-do-i-solve-this.352420/

        //Notify the disc who was the thrower
        DiscDamageScript dmgScript = netDisc.GetComponent<DiscDamageScript>();
        dmgScript.thrower = gameObject.GetComponent<NetworkIdentity>().netId;

        NetworkServer.Spawn(netDisc);
        Destroy(netDisc, resetTime);
    }

    [Command]
    private void Cmd_Grab(bool isLeftController) {
        Debug.Log("Telling client to grab");
        Rpc_ClientGrab(isLeftController);
    }

    [ClientRpc]
    private void Rpc_ClientGrab(bool isLeftController) {
        Debug.Log("Server told me to grab");
        //Set controller based on which called
        GameObject attachPoint;
        if ( isLeftController ) {
            attachPoint = leftControllerAttachPoint;
        } else {
            attachPoint = rightControllerAttachPoint;
        }

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

    [Command]
    private void Cmd_Release(bool isLeftController, Vector3 velocity, Vector3 angularVelocity) {
        Rpc_ClientRelease(isLeftController, velocity, angularVelocity);
    }

    [ClientRpc]
    private void Rpc_ClientRelease(bool isLeftController, Vector3 velocity, Vector3 angularVelocity) {
        //Set controller based on which called
        GameObject attachPoint;
        SteamVR_Controller.Device device;
        if ( isLeftController ) {
            attachPoint = leftControllerAttachPoint;
            device = Steam_LeftController;
        } else {
            attachPoint = rightControllerAttachPoint;
            device = Steam_RightController;
        }

        FixedJoint joint = attachPoint.GetComponent<FixedJoint>();

        if ( joint != null ) {
            if ( joint.connectedBody == null ) {
                DestroyImmediate(joint);
                return;
            }
            //Retrieve the interactable from the object
            GameObject releasedGo = joint.connectedBody.gameObject;
            InteractablePhysicalObject releasedInteractable = releasedGo.GetComponent<InteractablePhysicalObject>();
            //Debug.Log("releasedInteractable=" + releasedInteractable);
            //Destroy the joint connecting the two gameObjects 'releasing' the object
            releasedInteractable.beingInteracted = false;
            DestroyImmediate(joint);

            joint = null;

            //Apply physics to the interactable as if it were carrying momentum if there were any from the attachpoint
            float velocityMultiplier, angularVelocityMultiplier;
            velocityMultiplier = releasedInteractable.throwMultiplier;
            angularVelocityMultiplier = releasedInteractable.angularThrowMultiplier;

            //Apply a force to the disc
            Rigidbody rb = releasedInteractable.GetComponent<Rigidbody>();

            //Debug.Log("velocity=" + velocity);

            rb.velocity = velocity * velocityMultiplier;
            rb.angularVelocity = angularVelocity * angularVelocityMultiplier;
        }
    }
}
