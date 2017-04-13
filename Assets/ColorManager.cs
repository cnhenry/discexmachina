using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ColorManager : NetworkBehaviour {
    
    public Color myPlayerColor;
    public GameObject t1, t2, i1, i2;
    public GameObject[] playerModels;

    private void Start() {
        
    }

    private void Update() {
        
    }

    [ClientRpc]
    void RpcUpdateColors() {
        if (!isLocalPlayer) {
            return;
        }

        t1.GetComponent<Text>().color = myPlayerColor;
        t2.GetComponent<Text>().color = myPlayerColor;
        i1.GetComponent<Image>().color = myPlayerColor;
        i2.GetComponent<Image>().color = myPlayerColor;

        foreach (GameObject model in playerModels) {
            model.GetComponent<Renderer>().material.color = myPlayerColor;
        }
    }

    [Command]
    public void CmdSetPlayerColor(Color c) { 
        if (!isLocalPlayer) {
            return;
        }

        myPlayerColor = c;
        RpcUpdateColors();
    }
}
