using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float speed = 4f;

	[SerializeField]
	Laser laserPrefab;

	[SerializeField]
	AudioClip laserSound;

    Vector3 position;
	Animator animator;
	BoxCollider2D my_collider;
	bool isDestroyed = false;
	Player player;
    AudioSource audioSource;

    public static readonly HashSet<Enemy> Enemies = new HashSet<Enemy>();

    private void Awake()
    {
        Enemies.Add(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = SelectSpawnPoint();
        player = GameObject.FindWithTag("Player").gameObject.GetComponent<Player>();
        animator = GetComponent<Animator>();
        my_collider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

		StartCoroutine (ShootLaser ());
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
            my_collider.enabled = false;
            animator.SetTrigger("Destroy");
            audioSource.Play();
            Destroy(gameObject, 3f);
            Enemies.Remove(this);
        }

        if (other.transform.CompareTag("Laser") && isDestroyed == false)
        {
			if (other.GetComponent<Laser> ().IsEnemyLaser) {
				return;
			} else {
				Destroy (other.gameObject);
				if (player != null) {
					player.AddToScore (10);
				}
				isDestroyed = true;
				speed = 1f;
				my_collider.enabled = false;
				animator.SetTrigger ("Destroy");
				audioSource.Play ();
				Destroy (gameObject, 3f);
                Enemies.Remove(this);
            }
        }

        if (other.transform.CompareTag("Missile") && isDestroyed == false)
        {
            Destroy(other.gameObject);
            if (player != null)
            {
                player.AddToScore(10);
            }
            isDestroyed = true;
            speed = 1f;
            my_collider.enabled = false;
            animator.SetTrigger("Destroy");
            audioSource.Play();
            Destroy(gameObject, 3f);
            Enemies.Remove(this);
        }
    }

    Vector3 SelectSpawnPoint()
    {
        return new Vector3(Random.Range(-9.5f, 9.5f), 8f, 0);
    }

	IEnumerator ShootLaser() {
		while (true) {
			yield return new WaitForSeconds (Random.Range (3f, 7f));

			Laser laser = Instantiate(laserPrefab, transform.position + new Vector3(0, -1f, 0), Quaternion.identity);
			laser.IsEnemyLaser = true;
			audioSource.PlayOneShot (laserSound, .1f);
		}
	}
}
