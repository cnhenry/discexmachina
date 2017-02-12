using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class NetworkSpawner : NetworkBehaviour {

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnDisc();
        }

    }

    [Command]
    void spawnDisc()
    {
        var disc = (GameObject)Instantiate(prefab);

        disc.GetComponent<Rigidbody>().velocity = disc.transform.forward * 6;

        NetworkServer.Spawn(disc);
    }

}
