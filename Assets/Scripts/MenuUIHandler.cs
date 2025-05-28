using TMPro;
# if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIHandler : MonoBehaviour
{
    // This class is responsible for handling the UI in the main menu of the game.
    // It will manage the display of buttons, settings, and other UI elements relevant to the main menu.
    // You can add references to UI elements here, such as buttons, text fields, etc.
    // For example:
    // public Button startButton;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] TextMeshProUGUI highScoreDisplay;
    [SerializeField] Button startGameButton;
    [SerializeField] Button clearDataButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.LoadGameData(); // Load game data when the menu starts
        GameManager.Instance.SetPlayerName(playerNameInput.text); // Set the player's name from the input field
        playerNameInput.text = GameManager.Instance.playerName; // Load the player's name from GameManager
        highScoreDisplay.text = "High Score: " + GameManager.Instance.playerName.ToString() + GameManager.Instance.highScore.ToString(); // Display the high score
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGameClicked()
    {
        // This method is called when the start game button is clicked.
        // It can be used to transition to the game scene or start the game logic.
        GameManager.Instance.LoadGameData();
        SceneManager.LoadScene(1);
    }

    public void OnClearDataClicked()
    {
        // This method is called when the clear data button is clicked.
        // It can be used to reset the game data, such as high scores and player name.
        GameManager.Instance.ClearGameData(); // Save current game data before clearing
        GameManager.Instance.playerName = string.Empty; // Clear player name
        GameManager.Instance.highScore = 0; // Reset high score
        highScoreDisplay.text = "High Score: 0"; // Update the display
    }

    public void OnExitClicked()
    {
        GameManager.Instance.SaveGameData(); // Save game data before exiting
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Stop playing in the editor
#else
        // This method is called when the exit button is clicked.
        // It can be used to quit the application or return to the main menu.
        Application.Quit(); // Quit the application
        // If running in the editor, you can use UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
