using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using UnityStandardAssets.ImageEffects;

public class NetworkedPlayerHelper : NetworkBehaviour {
    public GameObject Hmd;
    public GameObject localHmd;
    public GameObject Controller;

    /// <summary>
    /// Called when local player authority has been assigned to a network object
    /// </summary>
    public override void OnStartLocalPlayer() {
        //Enable all cameras and listeners for the local player
        foreach ( Camera cam in GetComponentsInChildren<Camera>() ) {
            cam.enabled = true;
        }
        GetComponentInChildren<AudioListener>().enabled = true;
        GetComponentInChildren<EdgeDetectionColor>().enabled = true;
        Debug.Log("GetComponentInChildren<EdgeDetectionColor>()= " + GetComponentInChildren<EdgeDetectionColor>().ToString());
        Debug.Log("GetComponentInChildren<EdgeDetectionColor>.enabled? " + GetComponentInChildren<EdgeDetectionColor>().enabled);
        if ( SteamVR.active ) {
            //Enable steam vr scripts for the local player
            //Enable the disc spawner for the local player
            //localHmd.SetActive(true);
            List<Component> allComponents = GetComponents<Component>().ToList();
            allComponents.AddRange(GetComponentsInChildren<Component>());
            foreach (
                MonoBehaviour currentComponent in
                    allComponents.Where(component => component.ToString().Contains("Steam") || component.ToString().Contains("Spawner")) ) {
                currentComponent.enabled = true;
            foreach (
                EdgeDetectionColor shader in allComponents.Where(component => component.ToString().Contains("Edge")) ) {
                    shader.enabled = true;
                    Debug.Log("shader= " + shader.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Used to check for non-local player as onstartclient is ambiguous
    /// </summary>
    void Start() {
        switch ( isLocalPlayer ) {
            case true:
            gameObject.name = gameObject.name.Replace("(Clone)", "") + "_localPlayer";
            break;

            case false:
            gameObject.name = gameObject.name.Replace("(Clone)", "") + "_clientPlayer";

            //Create HMD and Controller objects
            foreach ( Transform childTransform in transform ) {
                CreateObjectFor(childTransform);
            }
            break;
        }
    }

    /// <summary>
    /// Create object model for transform
    /// </summary>
    /// <param name="tf"></param>
    private void CreateObjectFor(Transform tf) {
        GameObject newGameObject = null;
        if ( tf.name.Contains("Controller") ) {
            //newGameObject = Instantiate(Controller);
            //newGameObject.transform.position = tf.position;
            //newGameObject.transform.parent = tf;
        } else if ( tf.name.Contains("head") ) {
            newGameObject = Instantiate(Hmd);
            newGameObject.transform.position = tf.FindChild("Camera (eye)").position;
            newGameObject.transform.parent = tf.FindChild("Camera (eye)");
        }

        if ( newGameObject == null ) return;
        tf.gameObject.SetActive(true);
    }

    /// <summary>
    /// For testing only! - simulate orientation of non-vr player
    /// </summary>
    void Update() {
    }
}
