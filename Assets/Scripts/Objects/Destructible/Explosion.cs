using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float explosionForce;
    public float explosionRadius;
    public float upwardsModifier;

	// Use this for initialization
	void Start () {
        AudioSource snd = gameObject.GetComponent<AudioSource>();
        snd.loop = false;
        snd.Play();
        Explode();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 5);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardsModifier);
            }
        }
    }
}
