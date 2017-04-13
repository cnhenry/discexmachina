using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ScoreboardDisplay : NetworkBehaviour {
    public Text[] time;
    public Image[] healthBars;
    public Text[] scores;
    public Text[] results;
    public GameObject wall;

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
            t.text = Math.Round(timeInSeconds, 1).ToString();
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

    public void UpdateEndGame(int playerIndex, bool tie) {
        wall.SetActive(true);
        if (tie) {
            results[0].text = "Tie!";
            results[1].text = "Tie!";
        } else {
            if ( playerIndex == 0 ) {
                results[0].color = Color.cyan;
                results[1].color = Color.cyan;
                results[0].text = "BLUE team wins!";
                results[1].text = "BLUE team wins!";
            } else if ( playerIndex == 1 ) {
                results[0].color = Color.red;
                results[1].color = Color.red;
                results[0].text = "RED team wins!";
                results[1].text = "RED team wins!";
            }
        }
    }
}
