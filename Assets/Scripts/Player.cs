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
    GameObject laserPrefab;
    [SerializeField]
    GameObject tripleShotPrefab;

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

    float fireTimer = -1f;

    float tripleShotTimer = -1f;
    float speedBuffTimer = -1f;

    bool isTripleShotActive = false;
    bool isShieldActive = false;

    UIManager uiManager;
    Transform shield;
    int score = 0;

    void Start()
    {
        transform.position = new Vector3(0, -2f, 0);
        shield = transform.Find("Shield");
        shield.gameObject.SetActive(false);
        speed = defaultSpeed;
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (uiManager == null)
        {
            Debug.Log("No UI Manager found!");
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandlePlayerMovement();

        HandleFiring();
    }

    void HandlePlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * Time.deltaTime * speed);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10.9f, 10.9f),
            Mathf.Clamp(transform.position.y, -5.3f, 7.3f), 0);

        //Vector3 adjustedPosition = transform.position;

        //if (transform.position.y >= 7.5f)
        //{
        //    adjustedPosition.y = -5.5f;
        //}
        //else if (transform.position.y <= -5.5f)
        //{
        //    adjustedPosition.y = 7.5f;
        //}

        //if (transform.position.x >= 11.3f)
        //{
        //    adjustedPosition.x = -11.3f;
        //}
        //else if (transform.position.x <= -11.3f)
        //{
        //    adjustedPosition.x = 11.3f;
        //}

        //transform.position = adjustedPosition;
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

    public void TakeDamage()
    {
        if (isShieldActive == true)
        {
            shield.gameObject.SetActive(false);
            isShieldActive = false;
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

    public void ActivateShield()
    {
        isShieldActive = true;
        shield.gameObject.SetActive(true);
    }

    public void AddToScore(int amount)
    {
        score += amount;
        uiManager.UpdateScoreText(score);
    }
}

