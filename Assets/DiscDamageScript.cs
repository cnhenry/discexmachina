using UnityEngine;
using UnityEngine.Networking;

public class DiscDamageScript : NetworkBehaviour {

    public NetworkInstanceId thrower;
    public float damageAmount;

    [Server]
    private void OnCollisionEnter(Collision collision) {
        if ( !isServer ) {
            return;
        }

        //TODO: Modify this method to play a sound when a collision is detected. Play it on client side to not stream sound data.
        RpcPlayCollision();

        if (collision.collider.tag != "Player") {
            return;
        }

        GameObject player = collision.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        //LOL - this is becasue the collider on the online player is nested so deep onto the torso. 

        if ( player.GetComponent<NetworkIdentity>().netId == thrower ) {
            return; //Found myself. I'm not allowed to hit myself...
        }

        //Found a player
        if ( player.GetComponent<PlayerHealth>() != null ) { //This is a valid player to damage
            PlayerHealth damagedPlayer = player.GetComponent<PlayerHealth>();
            damagedPlayer.TakeDamage(damageAmount); // Do damage of the disc

            //Destroy the disc on player collision to prevent calls multiple times to this
            Destroy(gameObject);
        }
    }

    //Example Template on how to play sounds when collision detected...
    [ClientRpc]
    private void RpcPlayCollision() {

    }
}
