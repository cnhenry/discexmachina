using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

//[RequireComponent(typeof(SteamVR_TrackedObject))]
public class NetworkSpawner : NetworkBehaviour {

    SteamVR_TrackedObject trackedObj;
    public GameObject prefab;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) == true)
        {
            CmdSpawnDisc();
        }

    }

    [Command]
    void CmdSpawnDisc()
    {
        GameObject disc = (GameObject)Instantiate(prefab, transform.position, transform.rotation);

        //disc.GetComponent<Rigidbody>().velocity = disc.transform.forward * 6;

        //spawn the disc on the clients
        NetworkServer.Spawn(disc);

        //destroy the disc after 2 seconds
        Destroy(disc, 2.0f);
    }
}
