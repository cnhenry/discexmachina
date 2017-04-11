using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    public const float maxHealth = 100.0f;

    public float currentHealth;

    public float respawnTime = 5.0f;

    private Vector3 graveyard = new Vector3(-17, -8, -7);
    private Vector3 spawnLocation;

    public AudioClip deathSound;
    public AudioClip damageSound;
    public AudioClip respawnSound;

    private AudioSource source;

    void PlaySound(AudioSource source, AudioClip clip) {
        if (source.clip != clip)
        {
            source.Stop();
        }
        source.clip = clip;
        source.loop = false;
        source.mute = false;
        source.Play();
    }

    [ServerCallback]
    void OnEnable() {
        RpcSetHealthToMax();
        source = GetComponent<AudioSource>();
    }

    [Client]
    private void Start() {
        ResetPlayer(gameObject.GetComponent<NetworkIdentity>().netId);
        spawnLocation = this.transform.position; //On startup remember where the player spawned.
        source = GetComponent<AudioSource>();
    }

    [Server]
    public void TakeDamage(float amount, NetworkInstanceId playerWhoDidDamageToMe) {
        // Notify client of damage
        RpcDamageNotify(amount);

        // Actually take damage
        currentHealth -= amount;

        RpcLogForServer(currentHealth);
        if ( currentHealth <= 0 ) {
            currentHealth = 0;

            RpcDie();

            //Find where the scoring script. Increment who killed him
            ScoringScript myScoringEngine = GameObject.FindObjectOfType<ScoringScript>();
            myScoringEngine.AddKill(playerWhoDidDamageToMe);
        }
    }

    [ClientRpc]
    void RpcLogForServer(float cur) {
        Debug.Log("This is your currentHealth: " + cur);
    }

    [ClientRpc]
    void RpcDamageNotify(float amount) {
        if (isLocalPlayer) {
            Debug.Log("You have taken " + amount + " of damage. Health remaining = " + currentHealth);

            //TODO: On client side, play sound of indication of being hit
            PlaySound(source, damageSound);
        }
    }

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer) {
            Debug.Log("You are respawning...");
            ResetPlayer(gameObject.GetComponent<NetworkIdentity>().netId);
            transform.position = spawnLocation; //Move back to location where they spawned

            PlaySound(source, respawnSound);
        }
    }

    [ClientRpc]
    void RpcSetHealthToMax() {
        currentHealth = maxHealth;
    }

    [Command]
    void ResetPlayer(NetworkInstanceId playerId) {
        RpcSetHealthToMax();

        //Find player in scene and update him in scoring engine
        ScoringScript se = FindObjectOfType<ScoringScript>();
        se.AddPlayer(playerId);
    }

    [ClientRpc]
    void RpcDie() {
        if (isLocalPlayer) {
            Debug.Log("You have died");

            //TODO: Play sounds here on death
            PlaySound(source, deathSound);

            transform.position = graveyard;
        }
        Invoke("RpcRespawn", respawnTime);
    }
}
