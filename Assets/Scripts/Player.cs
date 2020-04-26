using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // PLAYER SPEED
    [SerializeField]
    float defaultSpeed = 5f;
    [SerializeField]
    float speedBuff = 2f;
    float speed;
    [SerializeField]
    float fireRate = .2f;

    // WEAPONS
    [SerializeField]
    Laser laserPrefab;
    [SerializeField]
    GameObject tripleShotPrefab;
    [SerializeField]
    GameObject heatMissilesPrefab;
    [SerializeField]
    int maxAmmo;
    int ammoCount;
    float fireTimer = -1f;

    // HEALTH & DAMAGE
    [SerializeField]
    GameObject shield;
    [SerializeField]
    int maxLives = 3;
    int lives;
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

    // BUFFS
    bool isTripleShotActive = false;
    int shieldStrength = 0;
    bool heatMissilesActive = false;

    // GENERAL
    [SerializeField]
    GameObject spawnManager;
    UIManager uiManager;
    Camera camera;

    int score = 0;

    void Start()
    {
        transform.position = new Vector3(0, -2f, 0);
        speed = defaultSpeed;
        lives = maxLives;
        ammoCount = maxAmmo;
        thrusterCharge = 5f;

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (uiManager == null)
        {
            Debug.Log("No UI Manager found!");
        } else
        {
            uiManager.UpdateAmmo(ammoCount);
            uiManager.UpdateScoreText(score);
        }
        
        audioSource = GetComponent<AudioSource>();
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

        Debug.Log("Thruster enabled: "+ thrusterEnabled);

        if (thrusterEnabled)
        {
            float translateSpeed = defaultSpeed * speedBuff * Time.deltaTime;
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * translateSpeed);
        }

        else
        {
            float translateSpeed = defaultSpeed * Time.deltaTime;
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * translateSpeed);
        }


        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10f, 10f),
            Mathf.Clamp(transform.position.y, -4f, 6f), 0);
    }

    void HandleFiring()
    {
        if(Input.GetKeyDown("space") && fireTimer < Time.time && ammoCount > 0)
        {
            fireTimer = Time.time + fireRate;

            if (heatMissilesActive == true)
            {
                Instantiate(heatMissilesPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                audioSource.PlayOneShot(laserSound, 1f);
            }
            else if (isTripleShotActive == true)
            {
                Instantiate(tripleShotPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
                audioSource.PlayOneShot(laserSound, .8f);
            } 
            else
            {
                Instantiate(laserPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                audioSource.PlayOneShot(laserSound, .4f);
            }

            ammoCount--;
            uiManager.UpdateAmmo(ammoCount);
        }
    }

    void HandleThrusters()
    {
        // TODO: Stop Coroutine and stop charging if on LeftShift keydown
        if (charging)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                charging = false;
            }

            if (thrusterCharge < maxCharge)
            {
                thrusterCharge += chargeRate * Time.deltaTime;
                uiManager.UpdateChargeDisplay(thrusterCharge / maxCharge);
            }
            else
            {
                charging = false;
            }
        }


        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            thrusterEnabled = false;
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

        else if (thrusterCharge <= 0)
        {
            thrusterEnabled = false;
            thrustersCo = StartCoroutine(RechargeThrusters(5f));
            return;
        }

        else if (Input.GetKey(KeyCode.LeftShift) && thrusterCharge > 0)
        {
            charging = false;
            thrusterEnabled = true;
            thrusterCharge -= chargeRate * Time.deltaTime;
            uiManager.UpdateChargeDisplay(thrusterCharge / maxCharge);
            thruster.SetActive(true);
        }

        else
        {
            Debug.Log("Final Condition");
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

            UpdateDamageVisuals(lives);

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

    public void RefillAmmo()
    {
        ammoCount = maxAmmo;
        uiManager.UpdateAmmo(ammoCount);
    }

    public void Heal()
    {
        if (lives < maxLives)
        {
            lives += 1;
        }

        UpdateDamageVisuals(lives);
        uiManager.UpdateLives(lives);
    }

    void UpdateDamageVisuals(int currentLives)
    {
        switch (currentLives)
        {
            case 3:
                for (int i = 0; i < damageField.Length; i++)
                {
                    damageField[i].SetActive(false);
                }
                break;
            case 2:
                damageField[Random.Range(0, 2)].SetActive(true);
                break;
            case 1:
                damageField[0].SetActive(true);
                damageField[1].SetActive(true);
                break;
            case 0:
                for (int i = 0; i < damageField.Length; i++)
                {
                    damageField[i].SetActive(true);
                }
                break;
            default:
                Debug.Log("Damage field visual update error -- lives out of expected range");
                break;
        }
    }

    IEnumerator HeatMissles()
    {
        heatMissilesActive = true;
        yield return new WaitForSeconds(5);
        heatMissilesActive = false;
    }

    public void ActivateHeatMissiles()
    {
        StartCoroutine(HeatMissles());
    }

    public void AddToScore(int amount)
    {
        score += amount;
        uiManager.UpdateScoreText(score);
    }
}

