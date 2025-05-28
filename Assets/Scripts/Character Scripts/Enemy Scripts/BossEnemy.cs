using UnityEngine;

public class BossEnemy : Enemy
{
    float SpecialAttackCooldown = 5; // Example property for a boss enemy
    // BossEnemy can have specific properties or methods that differentiate it from other enemies
    public override void Attack()
    {
        // Implement the attack logic for BossEnemy
        Debug.Log("BossEnemy attacks with a powerful strike!");
    }
    public override void Move()
    {
        // Implement the movement logic for BossEnemy
        Debug.Log("BossEnemy moves with a heavy stomp!");
    }
    public override void TakeDamage(int damageAmount)
    {
        // Implement the damage logic for BossEnemy
        base.TakeDamage(damageAmount); // Call the base class method to handle health reduction and death logic
        Debug.Log($"BossEnemy takes {damageAmount} damage!");
    }

    public override void ChaseTarget(Vector2 targetPosition)
    {
        // Implement the chase logic for BossEnemy
        Debug.Log($"BossEnemy chases target at position {targetPosition}!");
        base.ChaseTarget(targetPosition); // Call the base class method to handle movement towards the target
    }

    public override void ChaseTarget(Transform targetTransform)
    {
        // Implement the chase logic for BossEnemy using a Transform
        Debug.Log($"BossEnemy chases target at position {targetTransform.position}!");
        base.ChaseTarget(targetTransform); // Call the base class method to handle movement towards the target
    }

    // BossEnemy can have specific properties or methods that differentiate it from other enemies
    // For example, it might have a unique attack method or movement pattern
    // Example of a special attack method for the boss
    public void PerformSpecialAttack()
    {
        if (SpecialAttackCooldown <= 0)
        {
            Debug.Log("BossEnemy performs a special attack!");
            SpecialAttackCooldown = 5f; // Reset cooldown
        }
        else
        {
            Debug.Log("Special attack is on cooldown!");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialization code for BossEnemy can go here
        // For example, setting initial health, attack damage, etc.
        float health = this.Health + 20; // Use the base class property with the current instance
        Debug.Log("BossEnemy initialized with health: " + health);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
