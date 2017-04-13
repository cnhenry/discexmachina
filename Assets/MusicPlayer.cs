using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public AudioSource MpPlayer;
    public AudioClip[] songList;
    // Use this for initialization
    void Start() {
        playNewSong();
    }

    private void Update() {
        //On keypress n play new random song
        if (Input.GetKeyDown(KeyCode.N)) {
            MpPlayer.Stop();
        }

        if ( Input.GetKeyDown(KeyCode.A) ) {
            volumeUp();
        }

        if ( Input.GetKeyDown(KeyCode.Z) ) {
            volumeDown();
        }
    }

    IEnumerator WaitForTrackTOend() {
        while ( MpPlayer.isPlaying ) {
            yield return new WaitForSeconds(0.01f);
        }
        playNewSong();
    }

    private void playNewSong() {
        MpPlayer.clip = songList[Random.Range(0, songList.Length)];
        MpPlayer.loop = false;
        MpPlayer.Play();
        StartCoroutine(WaitForTrackTOend());
    }

    public void volumeUp() {
        MpPlayer.volume = MpPlayer.volume + 0.05f;
    }

    public void volumeDown() {
        MpPlayer.volume = MpPlayer.volume - 0.05f;
    }

    public void newSong() {
        MpPlayer.Stop();
    }
}
