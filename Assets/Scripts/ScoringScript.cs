using UnityEngine;
using UnityEngine.Networking;

public class ScoringScript : NetworkBehaviour {
    
    public float lengthOfRoundTime; //Time of the round in seconds

    [SyncVar]
    private float timeRemaining = 1;
    private float timeRunning, timeEnd;
    private static int numPlayers = 2;
    private int currentPlayers = 0;

    private int[] playerKills;
    private NetworkInstanceId[] myPlayers;
    private float[] playerHealth;

    ScoreboardDisplay sd;

    // Use this for initialization
    void Start () {
        sd = GameObject.FindObjectOfType<ScoreboardDisplay>();
        if ( isServer ) {
            currentPlayers = 0;
            playerKills = new int[numPlayers];
            myPlayers = new NetworkInstanceId[numPlayers];
            playerHealth = new float[numPlayers];
            timeRunning = Time.time;
            timeEnd = Time.time + lengthOfRoundTime;
        }   
    }
	
	// Update is called once per frame
	void Update () {
        if ( isServer ) {
            if ( timeRemaining > 0 ) {
                timeRunning = timeRunning + Time.deltaTime;
                timeRemaining = timeEnd - timeRunning;
            }
        }
        sd.UpdateScoreboardTime(timeRemaining);
    }

    [Server]
    public void AddPlayer(NetworkInstanceId playerId) {
        for (int i = 0 ; i < myPlayers.Length ; i++ ) {
            if (myPlayers[i] == playerId) {
                return;
            }
        }
        myPlayers[currentPlayers] = playerId;
        currentPlayers++;
    }

    //Function to modify health based on who was hit
    [Server]
    public void UpdateHealthOnPlayer(NetworkInstanceId playerId, float healthRemaining) {
        //Keep track of health on player, by calling this anytime health is modified.
        for ( int i = 0 ; i < myPlayers.Length ; i++ ) {
            if ( myPlayers[i] == playerId ) {
                playerHealth[i] = healthRemaining;
                Rpc_UpdateHealth(i, healthRemaining);
                return;
            }
        }
    }

    [ClientRpc]
    public void Rpc_UpdateHealth(int playerIndex, float health) {
        sd.Rpc_UpdatePlayerHealth(playerIndex, health);
    }

    [Server]
    public void AddKill(NetworkInstanceId playerId) {
        for (int i = 0 ; i < myPlayers.Length ; i++) { 
            if (myPlayers[i] == playerId ) {
                playerKills[i]++;
                Rpc_AddKill(i, playerKills[i]);
                return;
            }
        }
    }

    [ClientRpc]
    public void Rpc_AddKill(int i, int newScore) {
        sd.Rpc_UpdatePlayerScores(i, playerKills[i]);
    }
}
