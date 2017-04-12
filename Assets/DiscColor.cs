using UnityEngine;
using UnityEngine.Networking;

public class DiscColor : NetworkBehaviour { 
    [Command]
    public void Cmd_setColor(Color color) {
        gameObject.GetComponent<Renderer>().material.color = color;
    }
}
