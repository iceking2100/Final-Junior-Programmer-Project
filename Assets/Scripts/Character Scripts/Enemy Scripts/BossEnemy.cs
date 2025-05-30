using UnityEngine;

public class BossEnemy : Enemy
{
    // BossEnemy can have specific properties or methods that differentiate it from other enemies

    [Header("Boss Enemy Properties")]
    [SerializeField] private float bossAttackCooldown = 2f; // Cooldown time for BossEnemy's attacks
    //[SerializeField] private float bossAttackSpeed = 1.5f; // Attack speed for BossEnemy
    [SerializeField] private float bossAttackDamage = 20; // Attack damage for BossEnemy
    [SerializeField] private float bossHealthModifier = 3f; // Health modifier for BossEnemy
    [SerializeField] private float bossSpeedModifier = 1.5f; // Speed modifier for BossEnemy
    [SerializeField] private float bossMaxHealth = 200f; // Maximum health for BossEnemy
    [SerializeField] private float bossAttackRange = 3f; // Attack range for BossEnemy
    //[SerializeField] private float bossChaseRange = 10f; // Range within which BossEnemy will chase the player
    [SerializeField] private float SpecialAttackCooldown = 5; // Example property for a boss enemy
    
    private int bossValue = 10; // Score value for defeating the BossEnemy

    protected override void Awake()
    {
        base.Awake(); // Always call base.Awake() first to ensure base class initialization
        // Initialize BossEnemy-specific properties
        AttackDamage = (int)(bossAttackDamage * bossHealthModifier); // Set attack damage based on modifier
        MaxHealth = bossMaxHealth; // Set maximum health for BossEnemy
        Health = MaxHealth; // Initialize health to max health
        moveSpeed *= bossSpeedModifier; // Adjust move speed based on modifier
        attackRange = bossAttackRange; // Set attack range for BossEnemy

        scoreValue *= bossValue; // Set score value for defeating the BossEnemy
        // Debug log to confirm initialization
        Debug.Log($"BossEnemy initialized with health: {Health} , attack damage: {AttackDamage}");
        isAlive = true; // BossEnemy starts alive
        // Ensure health slider is set up correctly
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth; // Set the maximum value of the health slider
            healthSlider.value = Health; // Initialize the health slider value

            healthSlider.enabled = true;
            healthSlider.gameObject.SetActive(true); // Ensure the health slider is active in the UI
            Debug.Log("BossEnemy health slider initialized successfully.");
        }
        else
        {
            Debug.LogError("Health Slider is not assigned for BossEnemy!", this);
        }
    }

    // FixedUpdate is good for physics-based movement like patrolling or chasing targets
    private void FixedUpdate()
    {
        if (isAlive)
        {
            // If the BossEnemy is meant to patrol, call the base patrol logic
            // from its Move() implementation, which is called in FixedUpdate.
            Move(); // This will call the overridden Move() below.
        }
    }


    public override void Attack()
    {
        // Implement the attack logic for BossEnemy
        // Check if the BossEnemy is alive before attacking
        if (!isAlive)
        {
            Debug.LogWarning("BossEnemy is dead and cannot attack!");
            return; // Exit if the boss is not alive
        }

        if (isAlive)
        {
            // Check if the attack is off cooldown
            if (bossAttackCooldown <= 0)
            {
                // Perform the attack
                Debug.Log("BossEnemy attacks with a powerful strike!");
                // Here you can implement the actual attack logic, such as dealing damage to the player
                // For example, you might check if the player is within attack range and apply damage
                if (Vector3.Distance(GameManager.Instance.PlayerTransform.position, transform.position) <= attackRange)
                {
                    // Assuming GameManager has a reference to the player and a method to apply damage
                    GameManager.Instance.PlayerObject.TakeDamage(AttackDamage);
                }
                else
                {
                    Debug.Log("Player is out of range for BossEnemy's attack!");
                }
                // Implement the attack logic for BossEnemy
                Debug.Log("BossEnemy attacks with a powerful strike!");
                // Reset the cooldown after the attack
                bossAttackCooldown = 2f; // Reset cooldown after attack
            }
            else
            {
                Debug.Log("BossEnemy's attack is on cooldown!");
            }
        }
    // Implement the attack logic for BossEnemy
    Debug.Log("BossEnemy attacks with a powerful strike!");
    }
    public override void Move()
    {
        // Check if the BossEnemy is alive before moving
        if (!isAlive)
        {
            Debug.LogWarning("BossEnemy is dead and cannot move!");
            return; // Exit if the boss is not alive
        }

        Vector3 targetPosition = GameManager.Instance.PlayerTransform.position; // Get the player's position

        // Corrected the condition to compare distances using Vector3.Distance
        if (isAlive && Vector3.Distance(transform.position, targetPosition) <= 10f)
        {
            // Implement the movement logic for BossEnemy
            ChaseTarget(targetPosition); // Call the chase method to move towards the player
        }
        else
        {
            Debug.LogWarning("BossEnemy cannot move because it is not within range!");
        }

        // Implement the movement logic for BossEnemy
        Debug.Log("BossEnemy moves with a heavy stomp!");
    }
    public override void TakeDamage(int damageAmount)
    {
        // Implement the damage logic for BossEnemy
        base.TakeDamage(damageAmount); // Call the base class method to handle health reduction and death logic
        Debug.Log($"BossEnemy takes {damageAmount} damage!");
        HealthUpdate(); // Update the health slider UI after taking damage
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
            // Implement the special attack logic for BossEnemy
            // This could be a powerful attack that deals more damage or has a special effect
            // For example, you might deal extra damage to the player or create an area of effect
            // area that damages nearby enemies
            // Assuming GameManager has a reference to the player and a method to apply damage
            // Long range attack example
            float attackRange = bossAttackRange * 2; // Define the range for the special attack

            if (Vector3.Distance(GameManager.Instance.PlayerTransform.position, transform.position) <= attackRange)
            {
                // If the player is within attack range, perform the special attack

                GameManager.Instance.PlayerObject.TakeDamage(AttackDamage);
            }
            else
            {
                Debug.Log("Player is out of range for BossEnemy's special attack!");
            }

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
