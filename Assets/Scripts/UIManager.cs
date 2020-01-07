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
    GameObject gameOverTextContainer;
    [SerializeField]
    GameObject restartTextContainer;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: " + 0;
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

    public void ShowGameOver()
    {
        restartTextContainer.SetActive(true);
        if (gameManager != null)
        {
            gameManager.SetGameOver();
        }
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            gameOverTextContainer.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
            gameOverTextContainer.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.6f));
        }

    }

}
