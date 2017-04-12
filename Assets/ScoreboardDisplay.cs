using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreboardDisplay : NetworkBehaviour {
    public Text[] time;
    public Image[] healthBars;
    public Text[] scores;

    // Use this for initialization
    void Start() {
    }

    [ClientRpc]
    public void Rpc_UpdatePlayerHealth(int playerIndex, float health) {
        Debug.Log("PlayerIndex is: " + playerIndex);
        Debug.Log("Health is: " + health);
        switch ( playerIndex ) {
            case 0:
                healthBars[0].fillAmount = health / 100;
                healthBars[1].fillAmount = health / 100;
            break;
            case 1:
                healthBars[2].fillAmount = health / 100;
                healthBars[3].fillAmount = health / 100;
            break;
            default:
            break;
        }
    }

    public void UpdateScoreboardTime(float timeInSeconds) {
        foreach (Text t in time) {
            t.text = timeInSeconds.ToString();
        }
    }

    [ClientRpc]
    public void Rpc_UpdatePlayerScores(int playerIndex, int newScore) {
        switch (playerIndex) {
            case 0:
                scores[0].text = newScore.ToString();
                scores[1].text = newScore.ToString();
            break;
            case 1:
                scores[2].text = newScore.ToString();
                scores[3].text = newScore.ToString();
            break;
            default:
            break;
        }
    }
}
