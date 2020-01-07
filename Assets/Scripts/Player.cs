using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float defaultSpeed = 5f;
    [SerializeField]
    float speedBuff = 2f;

    float speed;

    [SerializeField]
    Laser laserPrefab;
    [SerializeField]
    GameObject tripleShotPrefab;

    [SerializeField]
    GameObject shield;

    [SerializeField]
    float fireRate = .2f;
    [SerializeField]
    int lives = 3;
    [SerializeField]
    GameObject spawnManager;

    [SerializeField]
    GameObject[] damageField;

    [SerializeField]
    GameObject explosion;

    [SerializeField]
    AudioClip laserSound;
    AudioSource audioSource;

    [SerializeField]
    GameObject thruster;
    [SerializeField]
    float thrusterSizeAdjustment = 1.5f;
    [SerializeField]
    float chargeRate = 1f;

    float fireTimer = -1f;

    float tripleShotTimer = -1f;
    float speedBuffTimer = -1f;

    bool isTripleShotActive = false;
    int shieldStrength = 0;

    float thrusterCharge = 0f;
    bool charging = false;
    bool thrustersActive = false;

    UIManager uiManager;
    int score = 0;

    void Start()
    {
        transform.position = new Vector3(0, -2f, 0);
        speed = defaultSpeed;
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (uiManager == null)
        {
            Debug.Log("No UI Manager found!");
        }
        audioSource = GetComponent<AudioSource>();

        thrusterCharge = 5f;
    }

    void Update()
    {

        Debug.Log("Shield Strength: " + shieldStrength);
        Debug.Log("Shield alpha: " + shield.GetComponent<SpriteRenderer>().material.color.a);
        HandleThrusters();
        HandlePlayerMovement();
        HandleFiring();
    }

    void HandlePlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (thrustersActive)
        {
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0)
               * Time.deltaTime * speed * speedBuff);
        }

        else
        {
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0) 
                * Time.deltaTime * speed);
        }


        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10f, 10f),
            Mathf.Clamp(transform.position.y, -4f, 6f), 0);
    }

    void HandleFiring()
    {
        if(Input.GetKeyDown("space") && fireTimer < Time.time)
        {
            fireTimer = Time.time + fireRate;

            if (isTripleShotActive == true)
            {
                Instantiate(tripleShotPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
                audioSource.PlayOneShot(laserSound, .8f);
            }
            else
            {
                Instantiate(laserPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                audioSource.PlayOneShot(laserSound, .4f);
            }
        }
    }

    void HandleThrusters()
    {
        Debug.Log(thrusterCharge);
        if (Input.GetKey(KeyCode.LeftShift) && thrusterCharge > 0 && !charging)
        {
            thrustersActive = true;
            thrusterCharge -= chargeRate * Time.deltaTime;
            thruster.SetActive(true);
            return;
        } else
        {
            thruster.SetActive(false);
        }

        if(thrusterCharge <= 0)
        {
            thrusterCharge = 0;
            StartCoroutine(RechargeThrusters());
        }

        if (charging)
        {
            if (thrusterCharge < 5)
            {
                thrusterCharge += chargeRate * Time.deltaTime;
            }
            else
            {
                charging = false;
            }
        }
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.CompareTag("Laser"))
		{
			if (other.GetComponent<Laser> ().IsEnemyLaser) {
				Destroy (other.gameObject);
				TakeDamage ();
			}
		}
	}


    public void TakeDamage()
    {
        if (shieldStrength >= 1)
        {
            shieldStrength -= 1;
            
            Color shieldColor = shield.GetComponent<SpriteRenderer>().material.color;
            
            switch (shieldStrength)
            {
                case 2:
                    shieldColor.a = .6f;
                    shield.GetComponent<SpriteRenderer>().material.color = shieldColor;
                    break;
                case 1:
                    shieldColor.a = .35f;
                    shield.GetComponent<SpriteRenderer>().material.color = shieldColor;
                    break;
                case 0:
                    shield.SetActive(false);
                    break;
                default:
                    Debug.Log("Something funny happened with the player shields");
                    break;
            }
            
            return;
        }

        if (lives >= 1)
        {
            lives--;
            uiManager.UpdateLives(lives);
            if (lives == 2)
            {
                damageField[Random.Range(0, 2)].SetActive(true);
            }
            else if (lives == 1)
            {
                damageField[0].SetActive(true);
                damageField[1].SetActive(true);
            } else if (lives == 0)
            {
                damageField[2].SetActive(true);
            }
        }

        else
        {
            SpawnManager spawner = spawnManager.GetComponent<SpawnManager>();
            if(spawner != null)
            {
                spawner.StopSpawning();
            }
            Instantiate(explosion, transform.position, Quaternion.identity);
            uiManager.ShowGameOver();
            Destroy(gameObject);
        }
    }

    public void ActivateTripleShot()
    {
        StartCoroutine(TripleShot());
    }

    IEnumerator TripleShot()
    {
        isTripleShotActive = true;
        yield return new WaitForSeconds(5);
        isTripleShotActive = false;
    }

    public void ActivateSpeed()
    {
        StartCoroutine(Speed());   
    }

    IEnumerator Speed()
    {
        speed = defaultSpeed * speedBuff;
        yield return new WaitForSeconds(5f);
        speed = defaultSpeed;
    }

    IEnumerator RechargeThrusters()
    {
        yield return new WaitForSeconds(5f);
        charging = true;
    }

    public void ActivateShield()
    {
        shieldStrength = 3;
        shield.SetActive(true);
        Color shieldColor = shield.GetComponent<SpriteRenderer>().material.color;
        shieldColor.a = 1f;
        shield.GetComponent<SpriteRenderer>().material.color = shieldColor;
    }

    public void AddToScore(int amount)
    {
        score += amount;
        uiManager.UpdateScoreText(score);
    }
}

