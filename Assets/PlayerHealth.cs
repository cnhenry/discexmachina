using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    public const float maxHealth = 100.0f;

    public float currentHealth;

    public float respawnTime = 5.0f;

    private Vector3 graveyard = new Vector3(-17, -8, -7);
    private Vector3 spawnLocation;

    [ServerCallback]
    void OnEnable() {
        RpcSetHealthToMax();
    }

    [Client]
    private void Start() {
        CmdSetHealthToMax();
        spawnLocation = this.transform.position;
    }

    [Server]
    public void TakeDamage(float amount) {
        //Notify client of damage
        RpcDamageNotify(amount);

        // Actually take damage
        currentHealth -= amount;

        RpcLogForServer(currentHealth);
        if ( currentHealth <= 0 ) {
            currentHealth = 0;
            RpcDie();
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
        }
    }

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer) {
            Debug.Log("You are respawning...");
            CmdSetHealthToMax();
            transform.position = spawnLocation; // move back to zero location
        }
    }

    [ClientRpc]
    void RpcSetHealthToMax() {
        currentHealth = maxHealth;
    }

    [Command]
    void CmdSetHealthToMax() {
        RpcSetHealthToMax();
    }

    [ClientRpc]
    void RpcDie() {
        if (isLocalPlayer) {
            Debug.Log("You have died");
            transform.position = graveyard;
        }
        Invoke("RpcRespawn", respawnTime);
    }
}
