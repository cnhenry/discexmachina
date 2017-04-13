using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    public const float maxHealth = 100.0f;

    public float currentHealth;

    public float respawnTime = 5.0f;

    private GameObject[] graveyard;
    private Vector3 spawnLocation;

    private int graveyardcounter;

    public AudioClip deathSound;
    public AudioClip damageSound;
    public AudioClip respawnSound, respawnCounter;
    public GameObject respawnAnimator;


    private AudioSource source;

    private NetworkInstanceId myNetID;

    void PlaySound(AudioSource source, AudioClip clip) {
        source.Stop();
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
        //Find graveyards in scene
        graveyardcounter = 0;
        graveyard = GameObject.FindGameObjectsWithTag("Graveyards");

        myNetID = gameObject.GetComponent<NetworkIdentity>().netId;
        CmdResetPlayer(myNetID);
        spawnLocation = this.transform.position; //On startup remember where the player spawned.
        source = GetComponent<AudioSource>();
    }

    [Server]
    public void TakeDamage(float amount, NetworkInstanceId playerWhoDidDamageToMe, NetworkInstanceId recieverOfDamage) {
        // Notify client of damage
        RpcDamageNotify(amount);

        // Actually take damage
        currentHealth -= amount;

        //Find where the scoring script, kill point!
        ScoringScript myScoringEngine = GameObject.FindObjectOfType<ScoringScript>();
        myScoringEngine.UpdateHealthOnPlayer(recieverOfDamage, currentHealth);

        RpcLogForServer(currentHealth);
        if ( currentHealth <= 0 ) {
            currentHealth = 0;
            RpcDie();
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

            //Find scoring engine, update player on health when damage is done
            //ScoringScript myScoringEngine = GameObject.FindObjectOfType<ScoringScript>();
            //myScoringEngine.UpdateHealthOnPlayer(myNetID, currentHealth);

            PlaySound(source, damageSound);
        }
    }

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer) {
            Debug.Log("You are respawning...");
            PlaySound(source, respawnSound);
            CmdResetPlayer(gameObject.GetComponent<NetworkIdentity>().netId);
            transform.position = spawnLocation; //Move back to location where they spawned

            //Find scoring engine, update player to full health
            //ScoringScript myScoringEngine = GameObject.FindObjectOfType<ScoringScript>();
            //myScoringEngine.UpdateHealthOnPlayer(myNetID, 100);
            respawnAnimator.SetActive(false);
        }
    }

    [ClientRpc]
    void RpcAnimationRespawn() {
        if (!isLocalPlayer) {
            return;
        }
        respawnAnimator.SetActive(true);
    }

    [ClientRpc]
    void RpcSetHealthToMax() {
        currentHealth = maxHealth;
    }

    [Command]
    void CmdResetPlayer(NetworkInstanceId playerId) {
        RpcSetHealthToMax();

        //Find player in scene and update him in scoring engine
        ScoringScript se = FindObjectOfType<ScoringScript>();
        se.AddPlayer(playerId);
        se.UpdateHealthOnPlayer(playerId, 100);
    }

    [ClientRpc]
    void RpcDie() {
        if (isLocalPlayer) {
            Debug.Log("You have died");

            //TODO: Play sounds here on death
            PlaySound(source, deathSound);

            transform.position = graveyard[graveyardcounter++].gameObject.transform.position;
            if(graveyardcounter >= graveyard.Length) {
                graveyardcounter = 0;
            }
        }

        // Respawn
        Invoke("RpcCounterRespawn", respawnTime - 3.0f);
        Invoke("RpcAnimationRespawn", respawnTime - 2.0f);
        Invoke("RpcRespawn", respawnTime);
    }

    [Client]
    void RpcCounterRespawn() {
        source.PlayOneShot(respawnCounter);
    }
}
