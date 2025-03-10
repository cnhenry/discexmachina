﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DestructibleSpawnObject : MonoBehaviour
{

    public GameObject objectToSpawn = null;
    public Vector3 spawnPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public Quaternion spawnRotation = Quaternion.identity;
    public GameObject[] whitelistedGameObjects;
    public AudioClip[] idleSndList;
    public List<string> gameObjectTags;
    private AudioClip idleSnd;

    // Use this for initialization
    void Start()
    {
        AudioSource snd = gameObject.GetComponent<AudioSource>();
        if (idleSndList.Length > 0)
        {
            int sndIndex = Random.Range(0, idleSndList.Length);
            idleSnd = idleSndList[sndIndex];
            snd.clip = idleSnd;
            snd.loop = true;
            snd.mute = false;
            snd.Play();
        }

        spawnPosition = gameObject.transform.position;
        foreach (GameObject go in whitelistedGameObjects)
        {
            if (!gameObjectTags.Contains(go.tag))
            {
                gameObjectTags.Add(go.tag);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (gameObjectTags.Contains(collision.gameObject.tag))
        {
            Instantiate(objectToSpawn, gameObject.transform.position, spawnRotation);
            Destroy(gameObject);
        }
    }
}
