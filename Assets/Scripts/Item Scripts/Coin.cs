using UnityEngine;
// This script defines a Coin class that inherits from Item

public class Coin : Item
{
    int points = 10; // Points awarded for collecting the coin
    // Override the OnCollected method to define what happens when the coin is collected
    // Coin-specific properties can be added here, if needed
    
    public override void OnCollected(Player player)
    {
        // Logic for what happens when the coin is collected
        GameManager.Instance.AddScore(points); // add score to the player
        Destroy(gameObject); // Remove the coin from the scene
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Move(); // Call the Move method to update the coin's position or animation
    }

    // You can add more methods or properties specific to the coin here if needed
    public void Move()
    {
        // Logic for moving the coin can be added here if needed
        // For example, you might want to make the coin move towards the player or follow a path
        // This is just a placeholder method for demonstration purposes
        // Update logic for the coin can be added here if needed
        // For example, you might want to rotate the coin or add animations
        transform.Rotate(Vector2.up, 50 * Time.deltaTime); // Example
        transform.Translate(0.001f * Mathf.Sin(Time.time) * Vector2.up); // Example of a simple bobbing effect

    }

}
