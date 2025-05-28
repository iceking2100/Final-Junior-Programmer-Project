using UnityEngine;

public class HealthPotion : Item
{
    public int healthAmount = 20; // Amount of health restored by the potion
    // Override the OnCollected method to define what happens when the potion is collected
    public override void OnCollected(Player player)
    {
        // Check if the player is alive before applying health
        if (player.isAlive)
        {
            player.health += healthAmount; // Restore health to the player
            Debug.Log($"Player collected a health potion and restored {healthAmount} health. Current health: {player.health}");
        }
        else
        {
            Debug.Log("Player is not alive, cannot collect health potion.");
        }
        Destroy(gameObject); // Remove the potion from the scene
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
