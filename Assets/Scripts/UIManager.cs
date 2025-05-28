using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    { get; private set; }

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

    // Method to show the main menu
    public void ShowMainMenu()
    {
        // Implement logic to display the main menu UI
        Debug.Log("Main Menu Displayed");

        // Example: Load the main menu scene or enable the main menu UI elements
        GameManager.Instance.SaveGameData(); // Save game data before exiting
        SceneManager.LoadScene(0); // Load the main menu scene
        // Or enable the main menu UI elements if they are part of the current scene
    }

    public void ShowConfigurator()
    {
        // Implement logic to display the configurator UI
    }

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
