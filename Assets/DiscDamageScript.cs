using UnityEngine;
using UnityEngine.Networking;

public class DiscDamageScript : NetworkBehaviour {

    public NetworkInstanceId thrower;
    public float damageAmount;

    public AudioClip reflectSound;
    private AudioSource source;

    [Server]
    public void SetThrower(NetworkInstanceId id) {
        thrower = id;
    }

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

        GameObject playerHit = collision.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        //LOL - this is becasue the collider on the online player is nested so deep onto the torso. 

        if ( playerHit.GetComponent<NetworkIdentity>().netId == thrower ) {
            return; //Found myself. I'm not allowed to hit myself...
        }

        //Found a player
        if ( playerHit.GetComponent<PlayerHealth>() != null ) { //This is a valid player to damage
            PlayerHealth damagedPlayer = playerHit.GetComponent<PlayerHealth>();
            damagedPlayer.TakeDamage(damageAmount, thrower); // Do damage of the disc

            //Destroy the disc on player collision to prevent calls multiple times to this
            Destroy(gameObject);
        }
    }

    void PlaySound(AudioSource source, AudioClip clip, float volume)
    {
        if (source.clip != clip)
        {
            source.Stop();
        }
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.mute = false;
        source.Play();
    }

    //Example Template on how to play sounds when collision detected...
    [ClientRpc]
    private void RpcPlayCollision() {
        source = GetComponent<AudioSource>();
        PlaySound(source, reflectSound, 0.5f);
    }
}
