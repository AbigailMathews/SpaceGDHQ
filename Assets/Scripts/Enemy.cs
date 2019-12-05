using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector3 position;
    Animator animator;
    BoxCollider2D collider;
    bool isDestroyed = false;

    [SerializeField]
    float speed = 4f;
    Player player;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = SelectSpawnPoint();
        player = GameObject.FindWithTag("Player").gameObject.GetComponent<Player>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -6f && isDestroyed == false)
        {
            transform.position = SelectSpawnPoint();
        }

        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && isDestroyed == false)
        {
            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer != null)
            {
                collidedPlayer.TakeDamage();
            }
            isDestroyed = true;
            speed = 1f;
            collider.enabled = false;
            animator.SetTrigger("Destroy");
            audioSource.Play();
            Destroy(gameObject, 3f);
        }

        if (other.transform.CompareTag("Laser") && isDestroyed == false)
        {
            Destroy(other.gameObject);
            if (player != null)
            {
                player.AddToScore(10);
            }
            isDestroyed = true;
            speed = 1f;
            collider.enabled = false;
            animator.SetTrigger("Destroy");
            audioSource.Play();
            Destroy(gameObject, 3f);
        }
    }

    Vector3 SelectSpawnPoint()
    {
        return new Vector3(Random.Range(-9.5f, 9.5f), 8f, 0);
    }
}
