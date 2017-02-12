using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SteamVR_NetworkSpawner : NetworkBehaviour {

    SteamVR_TrackedObject trackedObj;
    public GameObject prefab;
    public bool triggerButtonDown = false;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_Controller.Device controller
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

    // Use this for initialization
    void Start()
    {
        //trackedObj = GetComponent();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        triggerButtonDown = controller.GetPressDown(triggerButton);
        if (triggerButtonDown)
        {
            //Debug.Log("Spawn");
            CmdSpawnDisc();
        }

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) == true)
        {
            CmdSpawnDisc();
        }

    }

    [Command]
    void CmdSpawnDisc()
    {
        var disc = (GameObject)Instantiate(prefab);

        //disc.GetComponent<Rigidbody>().velocity = disc.transform.forward * 6;

        //spawn the disc on the clients
        NetworkServer.Spawn(disc);

        //destroy the disc after 2 seconds
        Destroy(disc, 2.0f);
    }
}
