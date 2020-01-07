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

    // AUDIO
    [SerializeField]
    AudioClip laserSound;
    AudioSource audioSource;

    // THRUSTERS
    [SerializeField]
    GameObject thruster;
    [SerializeField]
    float maxCharge = 5f;
    [SerializeField]
    float chargeRate = 1f;
    float thrusterCharge = 0f;
    bool charging = false;
    bool thrusterEnabled = false;
    Coroutine thrustersCo;

    float fireTimer = -1f;

    // BUFFS
    float tripleShotTimer = -1f;
    float speedBuffTimer = -1f;
    bool isTripleShotActive = false;
    int shieldStrength = 0;

    UIManager uiManager;
    int score = 0;

    Camera camera;

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

        camera = Camera.main;

        thrustersCo = StartCoroutine(RechargeThrusters(1f));
    }

    void Update()
    {
        HandleThrusters();
        HandlePlayerMovement();
        HandleFiring();
    }

    void HandlePlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (thrusterEnabled)
        {
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0)
               * Time.deltaTime * defaultSpeed * speedBuff);
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
        // TODO: Stop Coroutine and stop charging if on LeftShift keydown

        if (Input.GetKey(KeyCode.LeftShift) && thrusterCharge > 0)
        {
            thrusterEnabled = true;
            thrusterCharge -= chargeRate * Time.deltaTime;
            uiManager.UpdateChargeDisplay(thrusterCharge / maxCharge);
            thruster.SetActive(true);
            return;
        } 
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (thrusterCharge <= 0)
            {
                thrustersCo = StartCoroutine(RechargeThrusters(5f));
            }
            else
            {
                thrustersCo = StartCoroutine(RechargeThrusters(2f));
            }
            return;
        }

        if (charging)
        {
            if (thrusterCharge < maxCharge)
            {
                thrusterCharge += chargeRate * Time.deltaTime;
                uiManager.UpdateChargeDisplay(thrusterCharge / maxCharge);
            }
            else
            {
                charging = false;
            }

            return;
        }
        

    }

    void EnableThrusters()
    {
        thrusterEnabled = true;
        thrusterCharge -= chargeRate * Time.deltaTime;
        uiManager.UpdateChargeDisplay(thrusterCharge / maxCharge);
        thruster.SetActive(true);
    }

    void DisableThrusters()
    {
        thrusterEnabled = false;
        thruster.SetActive(false);
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

            StartCoroutine(camera.GetComponent<CameraEffects>().Shake());
        }

        else
        {
            SpawnManager spawner = spawnManager.GetComponent<SpawnManager>();
            if(spawner != null)
            {
                spawner.StopSpawning();
            }

            StartCoroutine(camera.GetComponent<CameraEffects>().Shake());

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

    IEnumerator RechargeThrusters(float time)
    {
        thruster.SetActive(false);
        yield return new WaitForSeconds(time);
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

