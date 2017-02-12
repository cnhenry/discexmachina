using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {
    public GameObject objectToSpawn;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;
    public int numberToSpawn;


    private int numberSpawned;

	// Use this for initialization
	void Start () {
        // Takes SpawnTime for first spawn
        // Takes SpawnTime for each subsequent spawn
        InvokeRepeating("Spawn", spawnTime, spawnTime);

        numberSpawned = 0;
	}
	
    void Spawn()
    {
        // Picks a random index between 0 and length of spawnPoints
        // (Random Spawnpoint)
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // Gets Size of SpawnPoint
        Vector3 scale = spawnPoints[spawnPointIndex].lossyScale;
        Vector3 radius = scale / 2 - objectToSpawn.transform.lossyScale;

        // Gets a random position relative to the origin
        Vector3 spawnPosition = new Vector3(Random.Range(-radius.x, radius.x), Random.Range(-radius.y, radius.y), Random.Range(-radius.z, radius.z));

        // Spawn the object within the spawn region with the random position
        Instantiate(objectToSpawn, spawnPoints[spawnPointIndex].position + spawnPosition, Quaternion.identity);
        numberSpawned++;

        if (numberSpawned == numberToSpawn)
        {
            CancelInvoke();
        }
    }
}
