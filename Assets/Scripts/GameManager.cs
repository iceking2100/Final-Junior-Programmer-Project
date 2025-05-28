using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int currentScore { get; set; } // Changed from private set to public set  
    public int playerLives;
    public int highScore;
    public string playerName;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddScore(int score)
    {
        currentScore += score;
    }
    public void LoseLife()
    {
        playerLives--;
    }
    public void LevelComplete(int level)
    {
        // Level completion logic  
        Debug.Log($"Level {level} completed! Current Score: {currentScore}, Player Lives: {playerLives}, High Score: {highScore}, Player Name: {playerName}");
        // You can add more logic here, such as saving the high score or transitioning to the next level
        if (currentScore > highScore)
        {
            highScore = currentScore; // Update high score if current score is higher
        }
        SaveGameData(); // Save game data after level completion
    }
    public void SetPlayerName(string name)
    {
        playerName = name;
    }
    public void LoadGameData()
    {
        // Load game data logic
        currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        playerLives = PlayerPrefs.GetInt("PlayerLives", 3); // Default to 3 lives if not set
        highScore = PlayerPrefs.GetInt("HighScore", 0); // Default to 0 if not set
        playerName = PlayerPrefs.GetString("PlayerName", "Player"); // Default name if not set
        // You can add more data loading logic here if needed
        // For example, loading player preferences, settings, etc.
    }
    public void SaveGameData()
    {
        // Save game data logic
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.SetInt("PlayerLives", playerLives);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }
    public void ClearGameData()
    {
        currentScore = 0;
        playerLives = 0;
        highScore = 0;
        playerName = string.Empty;
        PlayerPrefs.DeleteAll(); // Clear all saved data
        PlayerPrefs.Save(); // Save the changes
        // You can also reset other game data here if needed
    }
}
