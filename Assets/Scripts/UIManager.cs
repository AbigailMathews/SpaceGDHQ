using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    Sprite[] lifeSprites;
    [SerializeField]
    Image lifeDisplay;
    [SerializeField]
    GameObject chargeBarDisplay;
    [SerializeField]
    TextMeshProUGUI ammoText;
    [SerializeField]
    GameObject ammoTextContainer;
    [SerializeField]
    GameObject gameOverTextContainer;
    [SerializeField]
    GameObject restartTextContainer;
    GameManager gameManager;
    Coroutine ammoFlickerCo = null;

    // Start is called before the first frame update
    void Start()
    {
        gameOverTextContainer.SetActive(false);
        restartTextContainer.SetActive(false);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void UpdateScoreText(int playerScore)
    {
        scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int lives)
    {
        lifeDisplay.sprite = lifeSprites[lives];
    }

    public void UpdateChargeDisplay(float charge)
    {
        var scale = chargeBarDisplay.transform.localScale;
        scale.x = charge;
        chargeBarDisplay.transform.localScale = scale;
    }

    public void UpdateAmmo(int ammo)
    {
        ammoText.text = "Ammo: " + ammo;
        if (ammo <= 0)
        {
            ammoFlickerCo = StartCoroutine(Flicker(ammoTextContainer));
        } else
        {
            if (ammoFlickerCo != null)
            {
                StopCoroutine(ammoFlickerCo);
            }
        }

    }

    public void ShowGameOver()
    {
        restartTextContainer.SetActive(true);
        if (gameManager != null)
        {
            gameManager.SetGameOver();
        }
        StartCoroutine(Flicker(gameOverTextContainer));
    }

    IEnumerator Flicker(GameObject flickerTarget)
    {
        while (true)
        {
            flickerTarget.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
            flickerTarget.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.6f));
        }

    }

}
