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
        UpdateLivesDisplay(GameManager.Instance.GetPlayerLives());
        UpdateScoreDisplay(GameManager.Instance.GetCurrentScore());
    }

    // Update is called once per frame  
    void Update()
    {

    }

    public void UpdateScoreDisplay(int score)
    {
        score = GameManager.Instance.GetCurrentScore(); // Get the current score from GameManager
        string PlayerName = GameManager.Instance.GetPlayerName(); // Get the player's name from GameManager
        string scoreString = score.ToString(); // Convert score to string for display
        scoreText.text = $"Score: {PlayerName} {scoreString}";
    }

    public void UpdateLivesDisplay(int lives)
    {
        lives = GameManager.Instance.GetPlayerLives();
        string livesString = lives.ToString(); // Convert lives to string for display
        livesText.text = $"Lives: {livesString}";
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth; // Assuming health is a float between 0 and 1
    }
}
