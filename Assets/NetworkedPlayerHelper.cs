using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedPlayerHelper : NetworkBehaviour {

    public GameObject HeadModel;
    public GameObject ControllerModel;

    //Called when local player authority has been assigned to netowrk object
    public override void OnStartLocalPlayer() {
        //Enable all cameras and listeners for local player
        foreach ( Camera cam in GetComponentsInChildren<Camera>() ) {
            cam.enabled = true;
        }
        GetComponentInChildren<AudioListener>().enabled = true;

        if ( SteamVR.active ) {
            //Enable SteamVR scripts for local player
            List<Component> allComponents = GetComponents<Component>().ToList();
            allComponents.AddRange(GetComponentsInChildren<Component>());
            foreach (
                MonoBehaviour currentComponent in
                    allComponents.Where(component => component.ToString().Contains("Steam")).Cast<MonoBehaviour>() ) {
                currentComponent.enabled = true;
            }
        } else {
            transform.position = transform.position + Vector3.up * 1.75F;
        }
    }

    // Use this for initialization
    // Check for non-local player as onstartclient is ambigous
    void Start () {
        if ( isLocalPlayer ) {
            gameObject.name = gameObject.name.Replace("(Clone)", "") + "_localPlayer";
        } else {
            gameObject.name = gameObject.name.Replace("(Clone)", "") + "_clientPlayer";

            //Create HMD and Controller objects
            foreach ( Transform childTransform in transform ) {
                CreateObjectFor(childTransform);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if ( isLocalPlayer && !SteamVR.active ) {
            GameObject otherPlayer = GameObject.Find("OnlinePlayer_clientPlayer");
            if ( otherPlayer != null ) {
                Transform hmd = transform.FindChild("Camera (head)");
                hmd.LookAt(otherPlayer.transform.FindChild("Camera (head)"));
                transform.FindChild("Controller (right)").position = hmd.transform.position - Vector3.up * 0.5f + hmd.transform.forward * 0.25f + hmd.transform.right * 0.25f;
                transform.FindChild("Controller (right)").rotation = hmd.rotation;
                transform.FindChild("Controller (left)").position = hmd.transform.position - Vector3.up * 0.5f + hmd.transform.forward * 0.25f - hmd.transform.right * 0.25f;
                transform.FindChild("Controller (left)").rotation = hmd.rotation;
            }
        }

    }

    //Creates models for the network transforms
    private void CreateObjectFor(Transform tf) {
        GameObject newGameObject = null;
        if ( tf.name.Contains("Controller") ) {
            newGameObject = Instantiate(ControllerModel);
        } else if ( tf.name.Contains("head") ) {
            newGameObject = Instantiate(HeadModel);
        }

        if ( newGameObject == null ) return;
        newGameObject.transform.position = tf.position;
        newGameObject.transform.parent = tf;
        tf.gameObject.SetActive(true);
    }
}
