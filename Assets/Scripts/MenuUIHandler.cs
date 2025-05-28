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
    [Header("UI Panels (Scene Specific)")]
    // These references must be assigned in the Inspector for the UI panels
    // that exist ONLY within the scene this MenuUIHandler is placed in.
    public GameObject mainMenuPanel;    // Drag your Main Menu GameObject here (e.g., in Title Screen scene)
    public GameObject settingsMenuPanel; // Drag your Settings Menu GameObject here (e.g., in Title Screen scene)

    [Header("UI Elements")]
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] TextMeshProUGUI highScoreDisplay;

    [Header("Buttons")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button clearDataButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button settingsMenuButton; // Button to open the settings menu
    [SerializeField] Button saveAndExitButton; // Button to save and exit

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogError("MenuUIhandler: UIManager.Instance is null. Is UIManager GameObject in the first scene and set to DontDestroyOnLoad?");
            enabled = false; // Disable this script if UIManager is not found
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("MenuUIHandler: GameManager.Instance is null. Is GameManager GameObject in the first scene and set to DontDestroyOnLoad?");
            enabled = false; // Disable this script if GameManager is not found
            return;
        }

        // Load game data and initialize UI elements
        GameManager.Instance.LoadGameData(); // Load game data when the menu starts

        if (playerNameInput == null)
        {
            playerNameInput.text = GameManager.Instance.GetPlayerName(); // Load the player's name from GameManager
            // Add listener for when the input field value changes
            playerNameInput.onEndEdit.AddListener(OnPlayerNameChanged);
        }
        else
        {
            Debug.LogWarning("MenuUIHandler: Player Name Input Field is not assigned!");
        }

        // Update the high score display
        UpdateHighScoreDisplay();

        GameManager.Instance.SetPlayerName(playerNameInput.text); // Set the player's name from the input field
        playerNameInput.text = GameManager.Instance.GetPlayerName(); // Load the player's name from GameManager
        highScoreDisplay.text = "High Score: " + GameManager.Instance.GetPlayerName() + " " + GameManager.Instance.GetHighScore(); // Display the high score
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIPanelsBasedOnState();
    }

    /// <summary>
    /// Updates the active state of UI panels based on the UIManager's current global menu state.
    /// </summary>
    private void UpdateUIPanelsBasedOnState()
    {
        if (UIManager.Instance == null) return; // UIManager might not be initialized yet

        // Activate/Deactivate UI panels based on the currentMenuState
        if (mainMenuPanel == null)
        {
            mainMenuPanel.SetActive(UIManager.Instance.currentMenuState == UIManager.MenuState.MainMenu);
        }
        if (settingsMenuPanel == null)
        {
            settingsMenuPanel.SetActive(UIManager.Instance.currentMenuState == UIManager.MenuState.Configurator);
        }
        // You can add more panels here as needed for different menu states
    }

    /// <summary>
    /// Updates the high score display text.
    /// </summary>
    private void UpdateHighScoreDisplay()
    {
        // This method updates the high score display text.
        // It can be called whenever the high score changes.
        if (highScoreDisplay != null && GameManager.Instance != null)
        {
            highScoreDisplay.text = "High Score: " + GameManager.Instance.GetPlayerName() + " " + GameManager.Instance.GetHighScore();
        }
        else
        {
            Debug.LogWarning("MenuUIHandler: High Score Display Text is not assigned!");
        }
    }

    public void OnPlayerNameChanged(string newName)
    {
        // This method is called when the player name input field is changed.
        // It updates the player's name in the GameManager's game data.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerName(newName);
            UpdateHighScoreDisplay(); // Update the high score display after changing the player name
        }
    }

    public void OnStartGameClicked()
    {
        // This method is called when the start game button is clicked.
        // It Requests the UIManager to transition to the Gameplay state and loads the game scene.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RequestGamePlay(); // Request the UIManager to transition to the Gameplay state
        }
        // Save the current player name before loading the game scene
        GameManager.Instance.ManualSaveGame();
        SceneManager.LoadScene(1);
    }

    public void OnSettingsMenuClicked()
    {
        // This method is called when the settings menu button is clicked.
        // It can be used to open a settings menu or display settings options.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RequestConfigurator(); // Request the UIManager to transition to the Configurator state
        }
    }

    public void OnSaveAndExitClicked()
    {
        // This method is called when the save and exit button is clicked.
        // It can be used to save the game data and return to the main menu.
        GameManager.Instance.SaveGameData(); // Save game data

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RequestMainMenu(); // Request the UIManager to transition to the Main Menu state
        }
    }

    public void OnClearDataClicked()
    {
        // This method is called when the clear data button is clicked.
        // It can be used to reset the game data, such as high scores and player name.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearGameData(); // Clear game data
            UpdateHighScoreDisplay(); // Update the high score display after clearing data
            if (playerNameInput != null)
            {
                playerNameInput.text = GameManager.Instance.GetPlayerName(); // Reset player name input field
            }
        }
    }

    public void OnExitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ManualSaveGame(); // Save game data before exiting
        }
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Stop playing in the editor
#else
        Application.Quit(); // Quit the application in a build
#endif
    }

    void OnDestroy()
    {
        // Clean up event listeners to prevent memory leaks when this GameObject is destroyed.
        if (playerNameInput != null)
        {
            playerNameInput.onEndEdit.RemoveListener(OnPlayerNameChanged); // Remove listener for player name input field
        }
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGameClicked); // Remove listener for start game button
        if (settingsMenuButton != null)
            settingsMenuButton.onClick.RemoveListener(OnSettingsMenuClicked); // Remove listener for settings menu button
        if (saveAndExitButton != null)
            saveAndExitButton.onClick.RemoveListener(OnSaveAndExitClicked); // Remove listener for save and exit button
        if (clearDataButton != null)
            clearDataButton.onClick.RemoveListener(OnClearDataClicked); // Remove listener for clear data button
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked); // Remove listener for exit button
        
        // Optionally, you can also unsubscribe from UIManager's state change events if they exist
    }
}
