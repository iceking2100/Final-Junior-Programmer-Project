using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIHandler : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public Slider healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        UpdateLivesDisplay(GameManager.Instance.playerLives);
        UpdateScoreDisplay(GameManager.Instance.currentScore);
    }

    // Update is called once per frame  
    void Update()
    {

    }

    public void UpdateScoreDisplay(int score)
    {
        GameManager.Instance.currentScore += score;
        scoreText.text = GameManager.Instance.currentScore.ToString();
    }

    public void UpdateLivesDisplay(int lives)
    {
        GameManager.Instance.playerLives += lives;
        livesText.text = GameManager.Instance.playerLives.ToString();
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth; // Assuming health is a float between 0 and 1
    }
}
