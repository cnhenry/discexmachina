using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyController : MonoBehaviour {

    private SteamVR_TrackedController device;

	// Use this for initialization
	void Start () {
        device = GetComponent<SteamVR_TrackedController>();
        device.TriggerClicked += Trigger;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void Trigger(object sender, ClickedEventArgs e)
    {
        Debug.Log("Trigger has been pressed!");
    }
}
