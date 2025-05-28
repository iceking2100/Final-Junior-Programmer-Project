using UnityEngine;

public class SlimeEnemy : Enemy
// Inherits from Enemy class, which is a subclass of Character
// This class represents a slime enemy in the game
// It can have specific behaviors and properties that differentiate it from other enemies
{
    float health;

    // SlimeEnemy can have specific properties or methods that differentiate it from other enemies
    // For example, it might have a unique attack method or movement pattern
    // You can also override methods from the Enemy class if needed
    public override void Attack()
    {
        // Implement the attack logic for SlimeEnemy
        // For example, it might have a unique attack animation or damage calculation
        Debug.Log("SlimeEnemy attacks!");
    }
    public override void Move()
    {
        // Implement the movement logic for SlimeEnemy
        // For example, it might move in a bouncing or sliding manner
        Debug.Log("SlimeEnemy moves!");
    }
    public override void TakeDamage(int damageAmount)
    {
        // Implement the damage logic for SlimeEnemy
        // For example, it might have a unique reaction to taking damage
        base.TakeDamage(damageAmount); // Call the base class method to handle health reduction and death logic
        Debug.Log($"SlimeEnemy takes {damageAmount} damage!");
    }
    public override void ChaseTarget(Vector2 targetPosition)
    {
        // Implement the chase logic for SlimeEnemy
        // For example, it might move towards the target position in a unique way
        Debug.Log($"SlimeEnemy chases target at position {targetPosition}!");
        base.ChaseTarget(targetPosition); // Call the base class method to handle movement towards the target
    }

    public override void ChaseTarget(Transform targetTransform)
    {
        // Implement the chase logic for SlimeEnemy using a Transform
        // For example, it might move towards the target transform in a unique way
        Debug.Log($"SlimeEnemy chases target at position {targetTransform.position}!");
        base.ChaseTarget(targetTransform); // Call the base class method to handle movement towards the target
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Initialization code for SlimeEnemy can go here
        // For example, setting initial health, attack damage, etc.
        
        health = this.Health + 10; // Use the base class property with the current instance
        Debug.Log("SlimeEnemy initialized with health: " + health);
    }
    // Update is called once per frame
    void Update()
    {
        // Update logic for SlimeEnemy can go here
        // For example, checking for player proximity, attacking, etc.
    }
}
