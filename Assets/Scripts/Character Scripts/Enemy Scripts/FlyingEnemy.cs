using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    // Inherits from Enemy class, which is a subclass of Character
    // This class represents a flying enemy in the game
    // It can have specific behaviors and properties that differentiate it from other enemies

    [Header("Flying Enemy Properties")]
    // Inherited: healthSlider, spriteRenderer, enemyRb, playerTargetTransform, isAlive
    // Inherited: moveSpeed (will be dynamically set), AttackDamage (will be set), scoreValue (will be set)
    // Inherited: MaxHealth (will be set)

    [SerializeField] private float flyingMaxHealth = 100f; // Maximum health for FlyingEnemy
    [SerializeField] private int flyingScoreValue = 8; // Score value for defeating the FlyingEnemy
    [SerializeField] private int flyingAttackDamage = 10; // Attack damage for FlyingEnemy

    [Header("Flying Enemy Movement Properties")]
    [SerializeField] private float flyingPatrolSpeed = 2.0f; // Speed for patrolling behavior
    [SerializeField] private float flyingChaseSpeed = 3.0f; // Speed for chasing behavior
    [SerializeField] private float flyingHoverHeight = 2.0f; // Height at which the FlyingEnemy hovers
    [SerializeField] private float flyingHoverRange = 5.0f; // Range within which the FlyingEnemy hovers
    private Vector2 initialHoverPosition; // Initial position for hovering behavior

    [Header("Flying Enemy Attack")]
    [SerializeField] private float flyingAttackRange = 3f; // Attack range for FlyingEnemy
    [SerializeField] private float flyingChaseRange = 12f; // Range within which FlyingEnemy will chase the player
    [SerializeField] private float flyingAttackCooldownDuration = 1.0f; // Cooldown time for FlyingEnemy's attacks
    private float attackCooldownTimer; // Internal timer for attack cooldown

    // Current state of the flying enemy's AI
    private enum FlyingEnemyState { Patrolling, Chasing, Attacking }
    private FlyingEnemyState currentFlyingState;

    // --- MonoBehaviour Lifecycle Methods ---

    protected override void Awake()
    {
        base.Awake(); // Always call base.Awake() first to ensure base class initialization

        // Override/initialize properties inherited from Character/Enemy
        MaxHealth = flyingMaxHealth * flyingHealthModifier; // Set max health for FlyingEnemy, with a bonus based on flyingHealthBonus
        SetHealth(MaxHealth); // Initialize health to max health
        scoreValue = flyingScoreValue;
        AttackDamage = flyingAttackDamage; // Set attack damage for FlyingEnemy

        //Initialize attack cooldown timer
        attackCooldownTimer = 0f;

        // Store initial position for hovering patrol
        initialHoverPosition = transform.position;
        // Set the initial state to Patrolling
        currentFlyingState = FlyingEnemyState.Patrolling;
        // Debug log to confirm initialization
        Debug.Log($"FlyingEnemy initialized with health: {Health}, attack damage: {AttackDamage}, score value: {scoreValue}");
        //Debug.Log("FlyingEnemy Awake called.");
    }

    // FixedUpdate is good for physics-based movement like patrolling or chasing targets
    void FixedUpdate()
    {
        if (isAlive)
        {
            // Flying enemies don't need ground checks or ledge checks
            // CheckIfGrounded(); // Call the base method to check if grounded, even if not used for flying enemies
            // CheckForLedge(); // Call the base method to check for ledges, even if not used for flying enemies

            CheckAIState(); // Check the current AI state and perform actions accordingly
            Move(); // Call the Move method to handle movement based on the current state
            // Update attack cooldown timer
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.fixedDeltaTime; // Decrease the cooldown timer
            }
        }
        else
        {
            // If not alive, ensure enemy stops moving
            if (enemyRb != null) enemyRb.linearVelocity = Vector2.zero;
        }
    }

    // --- Core AI logic for FlyingEnemy ---
    // OVERRIDE: This method defines the unique decision-making logic for the FlyingEnemy.
    public override void CheckAIState()
    {
        // Ensure player target is available
        if (playerTargetTransform == null && GameManager.Instance != null)
        {
            playerTargetTransform = GameManager.Instance.PlayerTransform; // Get the player's transform from the GameManager
        }

        if (playerTargetTransform == null)
        {
            currentFlyingState = FlyingEnemyState.Patrolling; // If no player target, default to patrolling
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);

        if (distanceToPlayer <= flyingAttackRange)
        {
            // If within attack range, switch to Attacking state
            currentFlyingState = FlyingEnemyState.Attacking;
        }
        else if (distanceToPlayer <= flyingChaseRange)
        {
            // If within chase range, switch to Chasing state
            currentFlyingState = FlyingEnemyState.Chasing;
        }
        else
        {
            // If out of chase range, switch to Patrolling state
            currentFlyingState = FlyingEnemyState.Patrolling;
        }
    }
    // --- Movement Logic for FlyingEnemy ---
    // OVERRIDE: This method defines how the FlyingEnemy moves based on its current state.
    public override void Move()
    {
        Vector2 targetMovePosition;
        float currentMoveSpeed;

        switch (currentFlyingState)
        {
            case FlyingEnemyState.Chasing:
                // Chase the player
                currentMoveSpeed = flyingChaseSpeed; // Set chase speed
                targetMovePosition = playerTargetTransform != null ? (Vector2)playerTargetTransform.position : (Vector2)transform.position;
                // since base.ChaseTarget has ground checks, we'll re-implement the actual movement here
                ApplyFlyingMovement(targetMovePosition, currentMoveSpeed);
                break;
            case FlyingEnemyState.Patrolling:
                currentMoveSpeed = flyingPatrolSpeed; // Set patrol speed
                targetMovePosition = GetHoverPatrolTarget(); // Get a point to hover around
                ApplyFlyingMovement(targetMovePosition, currentMoveSpeed);
                break;
            case FlyingEnemyState.Attacking:
                // When attacking, the FlyingEnemy should not move, but it can still check for attack conditions
                currentMoveSpeed = 0f; // Stop movement
                if (enemyRb != null) enemyRb.linearVelocity = Vector2.zero; // Ensure the Rigidbody2D does not move
                Attack(); // Call the attack method to perform the attack logic
                break;
        }
    }

    // Helper method to apply flying movement logic
    private void ApplyFlyingMovement(Vector2 targetPos, float speed)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized; // Calculate the direction to the target position

        if (enemyRb != null)
        {
            // Flying enemies can move freely in the air.
            enemyRb.linearVelocity = direction * speed;
            // No clamping here for vertical speed, but can be added if needed
            enemyRb.linearVelocity = new Vector2(Mathf.Clamp(enemyRb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
                enemyRb.linearVelocity.y // Clamp vertical speed if needed
            );
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
            Debug.LogError("Rigidbody2D is not assigned for FlyingEnemy!", this);
        }

        // Flip sprite to face movement direction(horizontal only)
        if (direction.x > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.y > 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipY = false;
        }
    }
    // New method for Flying Enemy's unique patrol behavior (hovering)
    private Vector2 GetHoverPatrolTarget()
    {
        // Hover around the initial position within a certain range and height
        // Example Simple ping-pong or just moving towards a fixed offset from initial point

        // For simple hover, let's make it orbit a point or move between two points
        // We can use a sine wave for vertical oscillation to simulate hovering
        float hoverYOffset = Mathf.Sin(Time.time * flyingPatrolSpeed) * flyingHoverHeight / 2f;
        Vector2 hoverTarget = initialHoverPosition;

        // Horizontal patrol within hover range
        // For simplicity, Just moves Horizontally within hover range
        if (movingForward)
        {
            hoverTarget.x = initialHoverPosition.x + flyingHoverRange / 2f;
        }
        else
        {
            hoverTarget.x = initialHoverPosition.x - flyingHoverRange / 2f;
        }

        // Sine wave offset for vertical bobbing.
        hoverTarget.y = initialHoverPosition.y + flyingHoverHeight + hoverYOffset;

        // Simple horizontal "ping-pong" within hover range
        if (Vector2.Distance(transform.position, hoverTarget) < 0.1f)
        {
            movingForward = !movingForward; // Switch direction
        }
        return hoverTarget;
    }

    // --- Attack Logic for Flying Enemy ---
    // OVERRIDE: This method defines the unique attack behavior for FlyingEnemy.
    public override void Attack()
    {
        // Check if the FlyingEnemy is alive before attacking
        if (!isAlive || playerTargetTransform == null)
        {
            return;
        }

        if (attackCooldownTimer <= 0) // Check Cooldown
        {
            // Check if the target is within attack range
            if (Vector2.Distance(transform.position, playerTargetTransform.position) <= flyingAttackRange)
            {
                Debug.Log($"FlyingEnemy attacks player for {flyingAttackDamage} damage!");
                // Deal damage to the player
                Player player = playerTargetTransform.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(flyingAttackDamage);
                }
                attackCooldownTimer = flyingAttackCooldownDuration; // Reset the attack cooldown duration
            }
            // else: If not in range, it means we entered the Attacking state but player moved out.
            // The AI state will shift back to Chasing if the player is still in chase range
        }
        else
        {
            // Debug.Log("FlyingEnemy attack on cooldown.");
        }
    }

    // --- Overriding TakeDamage (and calling base) ---
    public override void TakeDamage(int damageAmount)
    {
        // Implement the damage logic for FlyingEnemy
        base.TakeDamage(damageAmount); // Call the base class method to handle health reduction and death logic
        // Debug.Log($"FlyingEnemy takes {damageAmount} damage!");
    }
    // --- ChaseTarget methods are overridden to call the specific ApplyFlyingMovement ---
    // we override these to ensure they use the flying movement logic, not the base's ground-based one
    public override void ChaseTarget(Vector2 targetPosition)
    {
        // This method will now internally call ApplyFlyingMovement directly
        // rather than relying on the base class's potentially grounded logic.
        ApplyFlyingMovement(targetPosition, flyingChaseSpeed);
        //Debug.Log($"FlyingEnemy chases target at position {targetPosition}!");
    }
    public override void ChaseTarget(Transform targetTransform)
    {
        if (targetTransform == null) return;
        ChaseTarget(targetTransform.position); // Call the Vector2 version
    }
}
