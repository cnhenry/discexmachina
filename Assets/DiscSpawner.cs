// Author - Cyrus Duong
// Date - 03-30-2017

using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class DiscSpawner : NetworkBehaviour {

    [SyncVar]
    private int playerNumber = 0;

    public GameObject discToSpawn;
    public GameObject leftController, rightController;
    public GameObject leftControllerAttachPoint, rightControllerAttachPoint;
    public float discDestroyTime, discRespawnTime;

    private float lastThrownDiscTime;
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

    private void OnConnectedToServer() {
        playerNumber++; //Increment player number to indicate color;
    }

    // Update is called once per frame
    void Update() {
        if ( !isLocalPlayer || !controllersGood ) { return; }

        if ( Steam_LeftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Left Grabbing");
            if ( Time.time - lastThrownDiscTime >= discRespawnTime ) {
                Cmd_Spawn(true, leftController.transform.position, leftController.transform.rotation);
            }
        }

        if ( Steam_LeftController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Left Released");
            Cmd_Release(true, Steam_LeftController.velocity, Steam_LeftController.angularVelocity);
        }

        if ( Steam_RightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Right Grabbing");
            if(Time.time - lastThrownDiscTime >= discRespawnTime) {
                Cmd_Spawn(false, rightController.transform.position, rightController.transform.rotation);
            }
        }

        if ( Steam_RightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) ) {
            //Debug.Log("Right Released");
            Cmd_Release(false, Steam_RightController.velocity, Steam_RightController.angularVelocity);
        }
    }

    // https://docs.unity3d.com/Manual/UNetSpawning.html
    // http://answers.unity3d.com/questions/987951/unet-networkserverspawn-not-working.html
    [Command]
    private void Cmd_Spawn(bool isLeft, Vector3 handPosition, Quaternion handRotation) {
        GameObject netDisc = Instantiate(discToSpawn, handPosition, handRotation);
        // https://forum.unity3d.com/threads/solved-clients-list-of-game-object-prefabs-not-shown-in-the-server-how-do-i-solve-this.352420/

        //Notify the disc who was the thrower
        DiscDamageScript dmgScript = netDisc.GetComponent<DiscDamageScript>();
        dmgScript.thrower = gameObject.GetComponent<NetworkIdentity>().netId;

        //Change color of throwing disc according to player number
        Material discMat = netDisc.transform.GetChild(0).GetComponent<Renderer>().material;
        switch (playerNumber) {
            case 0:
                discMat.color = Color.cyan;
            break;
            case 1:
                discMat.color = Color.red;
            break;
        }
        
        NetworkServer.Spawn(netDisc);
        Destroy(netDisc, discDestroyTime);

        Rpc_ClientGrab(isLeft, netDisc.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    private void Cmd_Grab(bool isLeftController, NetworkInstanceId discID) {
        //Debug.Log("Telling client to grab");
        Rpc_ClientGrab(isLeftController, discID);
    }

    [ClientRpc]
    private void Rpc_ClientGrab(bool isLeftController, NetworkInstanceId discID) {
        //Debug.Log("Server told me to grab");
        //Set controller based on which called
        GameObject attachPoint;
        if ( isLeftController ) {
            attachPoint = leftControllerAttachPoint;
        } else {
            attachPoint = rightControllerAttachPoint;
        }

        //Get list of interactables in the environment
        InteractablePhysicalObject[] interactables = GameObject.FindObjectsOfType(typeof(InteractablePhysicalObject)) as InteractablePhysicalObject[];

        //Search for given netID
        InteractablePhysicalObject spawnedInteractable = null;
        foreach ( InteractablePhysicalObject ipo in interactables ) {
            if ( ipo.GetComponent<NetworkIdentity>().netId == discID ) {
                spawnedInteractable = ipo;
                break;
            }
        }

        if ( spawnedInteractable != null ) {
            Debug.Log("Found spawned Interactable");
            // Flag interactable as active.
            spawnedInteractable.beingInteracted = true;

            // Set the position of the interactable and joint to rigidbody attachpoint on controllers
            spawnedInteractable.gameObject.transform.position = attachPoint.transform.position + spawnedInteractable.grabOffsetPosition;
            spawnedInteractable.gameObject.transform.rotation = attachPoint.transform.rotation * Quaternion.Euler(spawnedInteractable.grabOffsetRotation);

            //Add a joint onto the attachpoint
            FixedJoint joint = attachPoint.AddComponent<FixedJoint>();

            //Connect it to the interactable's rigidbody
            joint.connectedBody = spawnedInteractable.gameObject.GetComponent<Rigidbody>();
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

            lastThrownDiscTime = Time.time;
        }
    }
}
