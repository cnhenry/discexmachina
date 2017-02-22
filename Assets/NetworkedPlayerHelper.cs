using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedPlayerHelper : NetworkBehaviour {

    public GameObject HeadModel;
    public GameObject ControllerModel;
    public GameObject DiscModel;

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
            foreach (
                MonoBehaviour currentComponent in
                    allComponents.Where(component => component.ToString().Contains("Spawner")).Cast<MonoBehaviour>() ) {
                currentComponent.enabled = true;
            }
            foreach (
                MonoBehaviour currentComponent in
                    allComponents.Where(component => component.ToString().Contains("Interaction")).Cast<MonoBehaviour>() ) {
                currentComponent.enabled = true;
            }
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

    }

    //Creates models for the network transforms
    private void CreateObjectFor(Transform tf) {
        GameObject newGameObject = null;
        if ( tf.name.Contains("Controller") ) {
            newGameObject = Instantiate(ControllerModel);
        } else if ( tf.name.Contains("head")) {
            newGameObject = Instantiate(HeadModel);
        }

        if ( newGameObject == null ) return;
        newGameObject.transform.position = tf.position;
        newGameObject.transform.parent = tf;
        tf.gameObject.SetActive(true);
    }
}
