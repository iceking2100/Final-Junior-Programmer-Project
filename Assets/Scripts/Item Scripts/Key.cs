using UnityEngine;

public class Key : Item
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnCollected(Player player)
    {
        player.HasKey = true; // Set the player's hasKey property to true
    }
}
