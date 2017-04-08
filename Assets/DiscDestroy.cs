using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DiscDestroy : NetworkBehaviour {
    [Command]
    public void CmdDestroyTimer(float time) {
        Destroy(gameObject, time);
    }
}
