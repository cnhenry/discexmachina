using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
    public GameObject HeadModel;
    public GameObject HeadLocator, LeftArmLocator, RightArmLocator;
    public GameObject SteamVRHeadset, LeftController, RightController, Rig;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if ( !isLocalPlayer ) {
            Rig.SetActive(false);
            return;
        }
        //Hide the headmodel for the local client
        HeadModel.SetActive(false);
        //Leave arms visable

        //Update the head location for all other clients to see
        HeadLocator.transform.position = SteamVRHeadset.transform.position;
        HeadLocator.transform.rotation = SteamVRHeadset.transform.rotation;

        //Update location of arms based on controller positions
        LeftArmLocator.transform.position = LeftController.transform.position;
        LeftArmLocator.transform.rotation = LeftController.transform.rotation;
        RightArmLocator.transform.position = RightController.transform.position;
        RightArmLocator.transform.rotation = RightController.transform.rotation;
    }
}
