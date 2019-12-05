using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    float speed = 3f;

    string PowerUpType;

    [SerializeField]
    AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        PowerUpType = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            // Collect Powerup
            AudioSource.PlayClipAtPoint(audioClip, transform.position, 1f);

            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch(PowerUpType)
                {
                    case "TripleShot":
                        player.ActivateTripleShot();
                        break;
                    case "Speed":
                        player.ActivateSpeed();
                        break;
                    case "Shield":
                        player.ActivateShield();
                        break;
                    default:
                        Debug.Log("Unidentified Powerup");
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
