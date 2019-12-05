using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    float spinSpeed = 100f;
    [SerializeField]
    GameObject asteroidExplosion;
    SpawnManager spawnManager;

    void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            GameObject explosion = Instantiate(asteroidExplosion, transform.position, Quaternion.identity);
            spawnManager.StartSpawning();
            Destroy(other.gameObject);
            Destroy(explosion, 2.7f);
            Destroy(gameObject, 0.2f);
        }
    }
}
