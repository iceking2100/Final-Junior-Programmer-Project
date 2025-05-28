using System;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // This 'gameData' object will hold the game state information
    // All game logic (score, lives, high score, player name) will directly modify properties of this object.
    public static GameData gameData; // Refers to the nested GameData class

    private const string saveFileName = "gameData.json";

    private string GetSaveFilePath()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, saveFileName);
    }
    private void Awake()
    {
        LoadGameData(); // Load game data at the start

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
        gameData.currentScore += score;
    }
    public void LoseLife()
    {
        gameData.playerLives--;
    }
    public void LevelComplete(int level)
    {
        // Level completion logic  
        Debug.Log($"Level {level} completed! Current Score: {gameData.currentScore}, Player Lives: {gameData.playerLives}, High Score: {gameData.highScore}, Player Name: {gameData.playerName}");
        // You can add more logic here, such as saving the high score or transitioning to the next level
        if (gameData.currentScore > gameData.highScore)
        {
            gameData.highScore = gameData.currentScore; // Update high score if current score is higher
        }

        SaveGameData(); // Save game data after level completion
    }
    public void SetPlayerName(string name)
    {
        gameData.playerName = name;
    }

    public string GetPlayerName()
    {
        return gameData.playerName;
    }

    public int GetCurrentScore()
    {
        return gameData.currentScore;
    }

    public int GetPlayerLives()
    {
        return gameData.playerLives;
    }

    public int GetHighScore()
    {
        return gameData.highScore;
    }
    
    public Vector2 GetPlayerCurrentPosition()
    {
        // Assuming you have a reference to the player's Rigidbody2D component
        Rigidbody2D playerRb = FindFirstObjectByType<Player>().GetComponent<Rigidbody2D>();
        Vector2 playerPos = playerRb.position;
        // This method can return the current position of the player in the game world
        // For example, you can use the player's transform position if you have a player GameObject
        // Here we return a default position, but you can modify it to get the actual player's position
        return new Vector2(playerPos.x, playerPos.y); // Default position, can be modified as needed
    }

    public Vector2 GetPlayerSpawnPosition()
    {
        // This method can return a predefined spawn position for the player
        // For example, you can set it to the center of the screen or a specific point in your game world
        return new Vector2(0, 0); // Default spawn position, can be modified as needed
    }

    public void RespawnPlayer()
    {
        // Logic to respawn the player at a specific position
        // For example, you can set the player's position to the spawn position
        Vector2 spawnPosition = GetPlayerSpawnPosition();
        Rigidbody2D playerRb = FindFirstObjectByType<Player>().GetComponent<Rigidbody2D>();
        playerRb.position = spawnPosition; // Set the player's position to the spawn position
        playerRb.linearVelocity = Vector2.zero; // Reset velocity to prevent movement issues after respawn
        Debug.Log("Player respawned at position: " + spawnPosition);
    }

    // --- Save/Load Logic (using JsonUtilities) ---

    /// <summary>
    /// Saves game data to a JSON file.
    /// </summary>
    /// 

    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        string filePath = GetSaveFilePath();

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Game data saved to {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game data to {filePath}: {e.Message}");
        }
    }

    /// <summary>
    /// Loads game data from a JSON file on disk and updates the current saveManager object.
    /// If no save file exists or loading fails, the current saveManager object will remain with its default values.
    /// </summary>

    public void LoadGameData() 
    { 
        string filePath = GetSaveFilePath();

        //Check if the save file exists
        if (File.Exists(filePath))
        {
            try
            {
                // Read the entire content of the file as a JSON string.
                string json = File.ReadAllText(filePath);
                // Deserialize the JSON string back into the saveManager object.
                gameData = JsonUtility.FromJson<GameData>(json);
                Debug.Log($"Game data loaded from {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game data from {filePath}: {e.Message}");
                gameData = new GameData(); // Initialize with default values if loading fails
            }
        }
        else
        {
            Debug.LogWarning($"Save file not found at {filePath}. Initializing with default values.");
            gameData = new GameData(); // Initialize with default values if file does not exist
        }
    }

    /// <summary>
    /// Clears the game save file from disk and resets the saveManager object to default values.
    /// </summary>
    public void ClearGameData()
    {
        string filePath = GetSaveFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath); // Delete the save file
                Debug.Log($"Game data cleared from {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to clear game data from {filePath}: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"No save file found at {filePath} to clear.");
        }

        gameData = new GameData(); // Reset saveManager to default values
        Debug.Log("Game data reset to default values.");
    }

    /// <summary>
    /// Restarts the game by resetting the saveManager object to default values and clearing the save file.
    /// </summary>
    public void ResetSaveManagerToDefaults()
    {
        gameData = new GameData(); // Reset saveManager to default values
        ClearGameData(); // Clear the save file
        Debug.Log("Game data reset to default values and save file cleared.");
    }

    /// <summary>
    /// Manually triggers a game save (e.g., for a "Save Game" button in UI).
    /// </summary>
    public void ManualSaveGame()
    {
        SaveGameData(); // Call the save method to save current game state
        Debug.Log("Game manually saved.");
    }

    // Method to initialize game data, can be called at the start of the game
    [Serializable]
    public class GameData
    {
        public int currentScore;
        public int playerLives;
        public int highScore;
        public string playerName;
        // Volume settings
        public float musicVolume;
        public float sfxVolume;

        public GameData()
        {
            currentScore = 0;
            playerLives = 3; // Default to 3 lives
            highScore = 0; // Default to 0
            playerName = "Player"; // Default name

            // Default volume settings
            musicVolume = 1.0f; // Default music volume
            sfxVolume = 1.0f; // Default sound effects volume
        }
    }
}
