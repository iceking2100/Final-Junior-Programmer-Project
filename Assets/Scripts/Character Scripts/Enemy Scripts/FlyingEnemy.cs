using UnityEngine;

public class FlyingEnemy : Enemy
{
    // FlyingEnemy can have specific properties or methods that differentiate it from other enemies
    public override void Attack()
    {
        // Implement the attack logic for FlyingEnemy
        Debug.Log("FlyingEnemy attacks!");
    }
    public override void Move()
    {
        // Implement the movement logic for FlyingEnemy
        Debug.Log("FlyingEnemy moves!");
    }
    public override void TakeDamage(int damageAmount)
    {
        // Implement the damage logic for FlyingEnemy
        base.TakeDamage(damageAmount); // Call the base class method to handle health reduction and death logic
        Debug.Log($"FlyingEnemy takes {damageAmount} damage!");
    }
    public override void ChaseTarget(Vector2 targetPosition)
    {
        // Implement the chase logic for FlyingEnemy
        Debug.Log($"FlyingEnemy chases target at position {targetPosition}!");
        base.ChaseTarget(targetPosition); // Call the base class method to handle movement towards the target
    }
    public override void ChaseTarget(Transform targetTransform)
    {
        // Implement the chase logic for FlyingEnemy using a Transform
        Debug.Log($"FlyingEnemy chases target at position {targetTransform.position}!");
        base.ChaseTarget(targetTransform); // Call the base class method to handle movement towards the target
    }
    // FlyingEnemy can have specific properties or methods that differentiate it from other enemies
    // For example, it might have a unique attack method or movement pattern

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialization code for FlyingEnemy can go here
        // For example, setting initial health, attack damage, etc.
        float health = this.Health + 5; // Use the base class property with the current instance
        Debug.Log("FlyingEnemy initialized.");

    }

    // Update is called once per frame
    void Update()
    {
        // Update logic for FlyingEnemy can go here
        // For example, checking for player proximity, attacking, etc.
    }
}
