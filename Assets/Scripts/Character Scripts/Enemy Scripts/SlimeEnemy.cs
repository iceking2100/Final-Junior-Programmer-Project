using UnityEngine;

public class SlimeEnemy : Enemy
// Inherits from Enemy class, which is a subclass of Character
// This class represents a slime enemy in the game
// It can have specific behaviors and properties that differentiate it from other enemies
{

    // SlimeEnemy can have specific properties or methods that differentiate it from other enemies
    [Header("Slime Enemy Properties")]
    [SerializeField] private float slimeSpeedMultiplier = 0.5f; // Speed of the slime enemy
    [SerializeField] private int slimeAttackDamage = 5; // Attack damage for SlimeEnemy
    [SerializeField] private float slimeAttackRange = 2f; // Attack range for SlimeEnemy
    [SerializeField] private float slimeChaseRange = 8f; // Range within which SlimeEnemy will chase the player
    [SerializeField] private float slimeAttackCooldown = 1.5f; // Cooldown time for SlimeEnemy's attacks
    [SerializeField] private int slimeValue = 5; // Score value for defeating the SlimeEnemy

    // SlimeEnemy can have specific properties or methods that differentiate it from other enemies
    // For example, it might have a unique attack method or movement pattern

    protected override void Awake()
    {
        base.Awake(); // Always call base.Awake() first to ensure base class initialization
        // Additional initialization for SlimeEnemy can go here
        // For example, setting specific properties or behaviors for SlimeEnemy
        // Initialize SlimeEnemy-specific properties
        moveSpeed *= slimeSpeedMultiplier; // Adjust move speed based on slimeSpeedMultiplier

        // Set the attack range, attack damage, and score value for SlimeEnemy
        attackRange = slimeAttackRange; // Set attack range for SlimeEnemy
        AttackDamage = AttackDamage + slimeAttackDamage; // Set attack damage for SlimeEnemy
        scoreValue = slimeValue * scoreValue; // Set score value for defeating the SlimeEnemy
        MaxHealth = 30 * slimeHealthBonus; // Set max health for SlimeEnemy, with a bonus based on slimeHealthBonus
        SetHealth(MaxHealth); // Initialize health to max health
        isAlive = true; // SlimeEnemy starts alive

        Debug.Log($"SlimeEnemy initialized with health: {Health}, attack damage: {AttackDamage}, move speed: {moveSpeed}"); // Debug log to confirm initialization
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
        // Check if the SlimeEnemy is alive before attacking
        if (!isAlive)
        {
            Debug.LogWarning("SlimeEnemy is dead and cannot attack!");
            return; // Exit if the slime is not alive
        }

        if (slimeAttackCooldown > 0)
        {
            Debug.Log("SlimeEnemy's attack is on cooldown!");
            return; // Exit if the attack is on cooldown
        }

        // Check if the player is within attack range
        if (moveSpeed < 0)
        {
            Debug.LogWarning("SlimeEnemy cannot attack while moving backwards!");
            return; // Exit if the slime is moving backwards
        }


        if (Vector3.Distance(GameManager.Instance.PlayerTransform.position, transform.position) <= slimeAttackRange)
        {
            // If the player is within attack range, perform the attack
            Debug.Log("SlimeEnemy attacks!");
            // Implement the attack logic here, such as dealing damage to the player
            GameManager.Instance.PlayerObject.TakeDamage(slimeAttackDamage); // Example of dealing damage to the player
        }
        else
        {
            Debug.Log("SlimeEnemy is too far away to attack!");
        }
        // Implement the attack logic for SlimeEnemy
        // For example, it might have a unique attack animation or damage calculation
        // Reset the attack cooldown
        slimeAttackCooldown = 1.5f; // Reset the attack cooldown to the specified value
        Debug.Log("SlimeEnemy attacks!");
    }
    public override void Move()
    {
        // Implement the movement logic for SlimeEnemy
        // For example, it might move in a bouncing or sliding manner
        ExecutePatrolMovement(); // Call the patrol movement logic from the Enemy base class

        // Check if the player is within a certain distance
        // If the player is within a certain distance, chase the player
        if (Vector3.Distance(GameManager.Instance.PlayerTransform.position, transform.position) <= slimeChaseRange)
        {
            // If the player is within a certain distance, chase the player
            ChaseTarget(GameManager.Instance.PlayerTransform); // Chase the player using the Player's Transform
        }

        Debug.Log("SlimeEnemy moves!");
    }
    public override void TakeDamage(int damageAmount)
    {
        // Implement the damage logic for SlimeEnemy
        // For example, it might have a unique reaction to taking damage
        base.TakeDamage(damageAmount); // Call the base class method to handle health reduction and death logic
        Debug.Log($"SlimeEnemy takes {damageAmount} damage!");

        HealthUpdate(); // Call the method to update the health UI after taking damage

    }
/*    public override void HealthUpdate()
    {
        // Update the health slider UI for SlimeEnemy
        HealthUpdate(); // Call the base class method to update the health slider
        Debug.Log($"SlimeEnemy health updated: {Health}/{MaxHealth}");
    }*/
    public override void ChaseTarget(Vector2 targetPosition)
    {
        // Implement the chase logic for SlimeEnemy
        // For example, it might move towards the target position in a unique way
        //Debug.Log($"SlimeEnemy chases target at position {targetPosition}!");
        base.ChaseTarget(targetPosition); // Call the base class method to handle movement towards the target
    }

    public override void ChaseTarget(Transform targetTransform)
    {
        // Implement the chase logic for SlimeEnemy using a Transform
        // For example, it might move towards the target transform in a unique way
        //Debug.Log($"SlimeEnemy chases target at position {targetTransform.position}!");
        base.ChaseTarget(targetTransform); // Call the base class method to handle movement towards the target
    }

    // Update is called once per frame
    void Update()
    {
        // Update logic for SlimeEnemy can go here
        // For example, checking for player proximity, attacking, etc.
    }
}
