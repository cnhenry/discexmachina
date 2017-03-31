using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class DiscSpawner : NetworkBehaviour {

    public GameObject discToSpawn;
    public GameObject leftController, rightController;

    private EVRButtonId triggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;
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

        Debug.Log("Left Index: " + (int)leftTrackedObj.index);
        Debug.Log("Right Index: " + (int)rightTrackedObj.index);

        Steam_LeftController = SteamVR_Controller.Input((int)leftTrackedObj.index);
        Steam_RightController = SteamVR_Controller.Input((int)rightTrackedObj.index);
    }

    // Update is called once per frame
    void Update() {
        if ( !isLocalPlayer ) {
            return;
        }

        if ( Steam_LeftController.GetPressDown(triggerButton) ) {
            Debug.Log("Left Pressed");
            Cmd_Spawn(leftController.transform.position, leftController.transform.rotation);
        }

        if ( Steam_RightController.GetPressDown(triggerButton) ) {
            Debug.Log("Right Pressed");
            Cmd_Spawn(rightController.transform.position, rightController.transform.rotation);
        }

    }

    //Request Server Spawn Disc at Origin
    [Command]
    public void Cmd_Test() {
        GameObject netDisc = (GameObject)Instantiate(discToSpawn);
        NetworkServer.Spawn(netDisc);
    }

    // https://docs.unity3d.com/Manual/UNetSpawning.html
    // http://answers.unity3d.com/questions/987951/unet-networkserverspawn-not-working.html
    // Command means when the client calls this function it is forwarded to the server and lets the server handle it
    [Command]
    public void Cmd_Spawn(Vector3 handPosition, Quaternion handRotation) {
        GameObject netDisc = (GameObject)Instantiate(discToSpawn, handPosition, handRotation);
        // https://forum.unity3d.com/threads/solved-clients-list-of-game-object-prefabs-not-shown-in-the-server-how-do-i-solve-this.352420/
        //ClientScene.RegisterPrefab(netDisc);
        //Spawn the disc on clients
        NetworkServer.Spawn(netDisc);
    }

}
