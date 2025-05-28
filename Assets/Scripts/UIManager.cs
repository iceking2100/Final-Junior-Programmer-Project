using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    { get; private set; }

    public enum MenuState
    {
        MainMenu,
        Configurator,
        GamePlay,
        PauseMenu,
        GameOver
    }

    public MenuState currentMenuState { get; private set; } = MenuState.MainMenu;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    // --- Public methods to handle UI state changes ---
    /// <summary>
    /// Requests the UI system to transition to the Main Menu state.
    /// Scene-specific UI handlers will react to this change.
    /// </summary>
    // Method to show the main menu
    public void RequestMainMenu()
    {
        currentMenuState = MenuState.MainMenu;
        Debug.Log("UIManager: Requested Main Menu state");
    }

    /// <summary>
    /// Requests the UI system to transition to the Configurator state.
    /// Scene-specific UI handlers will react to this change.
    /// </summary>
    public void RequestConfigurator()
    {
        currentMenuState = MenuState.Configurator;
        // Implement logic to display the configurator UI
    }

    /// <summary>
    /// Requests the UI system to transition to the Gameplay state.
    /// Scene-specific UI handlers will react to this change and typically load the game scene.
    /// </summary>
    public void RequestGamePlay()
    {
        currentMenuState = MenuState.GamePlay;
        Debug.Log("UIManager: Requested Gameplay state");
    }

    /// <summary>
    /// Requests the UI system to transition to the Pause Menu state.
    /// Scene-specific UI handlers will react to this change and typically show the pause menu UI.
    /// </summary>
    public void ShowPauseMenu()
    {
        currentMenuState = MenuState.PauseMenu;
        Debug.Log("Pause Menu Displayed");
    }

    /// <summary>
    /// Requests the UI system to transition to the Game Over state.
    /// Scene-specific UI handlers will react to this change and typically show the game over UI.
    /// </summary>
    public void ShowGameOver()
    {
        currentMenuState = MenuState.GameOver;
        Debug.Log("Game Over Displayed");
    }

    // Utility methods for UI interactions

    public void DisplayMessage(string message)
    {
        // Implement logic to display a message to the user
        // Debug.Log(message);
    }

    public void UpdatePlayerNameDisplay(string name)
    {
        // Implement logic to update the player's name display in the UI
        Debug.Log("Player Name Updated: " + name);
        // For example, you might update a Text or TextMeshPro component in the UI
        // TextMeshProUGUI playerNameText = FindObjectOfType<TextMeshProUGUI>();
        // if (playerNameText != null)
    }
}
