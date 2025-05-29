using UnityEngine;

public class SlimeEnemy : Enemy
// Inherits from Enemy class, which is a subclass of Character
// This class represents a slime enemy in the game
// It can have specific behaviors and properties that differentiate it from other enemies
{

    // SlimeEnemy can have specific properties or methods that differentiate it from other enemies
    [SerializeField] private float slimeSpeedMultiplier = 0.5f; // Speed of the slime enemy
    [SerializeField] private float slimeHealthBonus = 1.5f; // Additional health for SlimeEnemy
    [SerializeField] private int slimeAttackDamage = 5; // Attack damage for SlimeEnemy

    protected override void Awake()
    {
        base.Awake(); // Always call base.Awake() first to ensure base class initialization
        // Additional initialization for SlimeEnemy can go here
        // For example, setting specific properties or behaviors for SlimeEnemy
        AttackDamage = 10 + slimeAttackDamage; // Set attack damage for SlimeEnemy
        ScoreValue = 100; // Set score value for SlimeEnemy
        MaxHealth = 30 * slimeHealthBonus; // Set max health for SlimeEnemy, with a bonus based on slimeHealthBonus
        Health = MaxHealth; // Initialize health to max health

        moveSpeed *= slimeSpeedMultiplier; // Adjust move speed based on slimeSpeedMultiplier
    }

    // FixedUpdate is good for physics-based movement like patrolling or chasing targets
    private void FixedUpdate()
    {
        if (isAlive)
        {
            // if the SlimeEnemy is meant to patrol, call the base patrol logic
            // from its Move() implementation, which is called in FixedUpdate.
            Move(); // This will call the overridden Move() below.
        }
    }


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
        ExecutePatrolMovement(); // Call the patrol movement logic from the Enemy base class

        // Or if the slime has completely different movement (e.g., chasing player always.) :
        // ChaseTarget(targetPosition); // Uncomment this if you want the slime to always chase a target
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

    // Update is called once per frame
    void Update()
    {
        // Update logic for SlimeEnemy can go here
        // For example, checking for player proximity, attacking, etc.
    }
}
