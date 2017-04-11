using UnityEngine;
using UnityEngine.Networking;

public class ScoringScript : NetworkBehaviour {
    
    public float lengthOfRoundTime; //Time of the round in seconds

    private float timeRunning, timeEnd, timeRemaining;
    private static int numPlayers = 2;
    private int currentPlayers = 0;
    private int[] playerKills = new int[numPlayers];
    private NetworkInstanceId[] myPlayers = new NetworkInstanceId[numPlayers];
    private float[] playerHealth = new float[numPlayers];

    // Use this for initialization
    void Start () {
        if (isServer) {
            timeRunning = Time.time;
            timeEnd = Time.time + lengthOfRoundTime;
        }   
    }
	
	// Update is called once per frame
	void Update () {
        if ( isServer ) {
            timeRunning = timeRunning + Time.deltaTime;
            timeRemaining = timeEnd - timeRunning;
        }
    }

    public void AddPlayer(NetworkInstanceId playerId) {
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
                return;
            }
        }
    }

    [Server]
    public void AddKill(NetworkInstanceId playerId) {
        for (int i = 0 ; i < myPlayers.Length ; i++) { 
            if (myPlayers[i] == playerId ) {
                playerKills[i]++;
                return;
            }
        }
    }

    [Server]
    public float[] GetPlayerHealth() {
        return playerHealth;
    }

    [Server]
    public int[] GetPlayerKills() {
        return playerKills;
    }
    //Time remaining function to call per update
    [Server]
    public float TimeRemaining() {
        //Some how calculate how much time there is left in a round
        return timeRemaining; //TODO
    }
}
