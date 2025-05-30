using System;
using UnityEngine;

public abstract class Enemy : Character
{
    // INHERITANCE: Enemy is a subclass of Character, which is an abstract class
    // This class represents a base class for all enemies in the game, inheriting from Character
    // ENCAPSULATION: Health, AttackDamage, and other properties are encapsulated within the class
    // and can be accessed or modified through public methods or properties.
    // ABSTRACTION: This class provides a common interface for all enemy types, defining abstract methods for attack and movement
    // POLYMORPHISM: Different enemy types can implement their own versions of attack and movement methods





    [Header("Enemy Type Settings")]
    [SerializeField] protected float flyingHealthModifier = 0.75f; // Additional health for FlyingEnemy
    [SerializeField] protected float slimeHealthBonus = 1.5f; // Additional health for SlimeEnemy

    [Header("Enemy Settings")]
    [SerializeField]
    protected int scoreValue = 100; // Score value for defeating this enemy, can be set in the inspector

    // References to components (protected for derived class access)
    protected SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component for visual effects (protected for derived class access)
    protected Rigidbody2D enemyRb;

    // --- AI Target & State Management ---
    protected Transform playerTargetTransform; // Reference to the target Transform (e.g., player) for chasing behavior
    protected Vector2 currentTargetPosition; // The actual position the enemy moves towards

    [Header("Chase Settings")] // header for chase-specific variables
    [SerializeField] protected float chaseRange = 5f; // Distance at which enemy starts chasing player
    [SerializeField] protected float loseChaseRange = 6f; // Distance at which enemy stops chasing
    [SerializeField] protected float maxHorizontalSpeed = 5f; // Max horizontal speed for the enemy (similar to player)
    [SerializeField] protected float turnBuffer = 0.1f; // For horizontal turning (e.g. ledge detection, patrol)

    // --- Patrol specific variables (protected for derived class access) ---
    [Header("Patrol Settings")]
    [SerializeField]
    protected Transform[] patrolWaypoints; // Array to hold multiple waypoints for the patrol path
    protected int currentWaypointIndex = 0;
    protected bool movingForward = true; // For ping-pong patrol type
    public enum PatrolType { Loop, PingPong }
    [SerializeField]
    protected PatrolType patrolStyle = PatrolType.Loop; // Default patrol style

    // --- Ground and Ledge Detection (Essential for grounded enemies) ---
    [Header("Grounded & Ledge Detection")]
    [SerializeField] protected Transform enemyGroundCheckPoint; // Point at enemy's feet for ground check
    [SerializeField] protected float enemyGroundCheckRadius = 0.1f; // Radius for ground check circle
    [SerializeField] protected LayerMask groundLayer; // LayerMask for ground objects
    [SerializeField] protected Transform ledgeCheckPoint; // point slightly ahead of enemys feet for ledge check
    [SerializeField] protected float ledgeCheckDistance = 0.2f; // Distance for ledge raycast

    protected bool isGrounded;
    protected Vector3 initialPatrolPosition; // Stores the enemy's position when it spawns for back-and-forth patrol

    // --- MonoBehaviour Lifecycle Methods ---

    protected override void Awake()
    {
        base.Awake(); // Always call base.Awake() first to ensure base class initialization
        enemyRb = GetComponent<Rigidbody2D>();

        if (enemyRb == null)
        {
            Debug.LogError($"Rigidbody2D component is missing on {gameObject.name}. Enemy movement might not work as expected with physics.", this);
        }

        // Initialize spriteRenderer for visual effects
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer component is missing on {gameObject.name}. Visual effects might not work as expected.", this);
        }

        // Initialize targetTransform to the player's transform from GameManager
        if (GameManager.Instance != null)
        {
            playerTargetTransform = GameManager.Instance.PlayerTransform;
            if (playerTargetTransform == null)
            {
                Debug.LogWarning("GameManager.PlayerTransform is not available yet. Enemy will not chase initially.", this);
            }
        }
        else
        {
            Debug.LogWarning("GameManager instance is not available. Enemy will not chase the player.", this);
        }

        // store initial position for back-and-forth patrol
        initialPatrolPosition = transform.position;


        // Validate waypoints ONLY IF patrolwaypoints is not null and length > 0
        // Specific enemies might not use patrol so don't disable if it's not assigned
        if (patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            // Initialize currentWaypointIndex for patrol
            currentWaypointIndex = 0;
        }

        //Validate ground/ledge check points
        if (enemyGroundCheckPoint == null) Debug.LogWarning($"EnemyGroundCheckPoint no assigned for {gameObject.name}", this);
        if (ledgeCheckPoint == null) Debug.LogWarning($"LedgeCheckPoint not assigned for {gameObject.name}", this);
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            CheckIfGrounded(); // Update grounded status
            CheckAIState(); // Determine current AI behavior (chase or patrol)
            // Move() is now called from CheckAIState based on behavior
        }
        else
        {
            // If not alive. ensure enemy stops moving
            if (enemyRb != null) enemyRb.linearVelocity = Vector3.zero;
        }
    }

    // --- Abstract methods from Character that MUST be implemented by specific enemy types ---
    public abstract override void Attack();
    public abstract override void Move();// This is implemented by derived classes to call ExecutePatrolMovement or ChaseTarget

    // --- Overriding TakeDamage from Character ---
    public override void TakeDamage(int damageAmount)
    {
        SetHealth(Health - damageAmount); // using protected setter
        // Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {Health}");

        HealthUpdate(); // Call the method to update the health UI after taking damage
        if (Health <= 0)
        {
            Die(); // will handle adding score
        }
    }

    // --- Polymorphism: Overloaded ChaseTarget Methods ---
    // These methods allow different ways to chase a target, either by position or by Transform.
    public virtual void ChaseTarget(Vector2 targetPos)
    {
        // Abstraction: This Method abstracts the movement calculations.
        // It should only apply horizontal movement if grounded.
        if (!isGrounded)
        {
            // if not grounded don't apply horizontal movement.
            if (enemyRb != null) enemyRb.linearVelocity = new Vector2(0, enemyRb.linearVelocity.y);
            return;
        }

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        //Only consider horizontal direction for movement.
        float horizontalDirection = Mathf.Sign(direction.x); // -1 for left, 1 for right

        // Apply movement using Rigidbody2D.linearVelocity
        if (enemyRb != null)
        {
            enemyRb.linearVelocity = new Vector2(horizontalDirection * moveSpeed, enemyRb.linearVelocity.y);
            // Clamp horizontal speed
            enemyRb.linearVelocity = new Vector2(
                Mathf.Clamp(enemyRb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
                enemyRb.linearVelocity.y
            );

        }
        else
        {
            // Fallback for direct transform movement if no Rigidbody (less ideal for physics characters)
            transform.position += (Vector3)(new Vector2(horizontalDirection, 0) * moveSpeed * Time.deltaTime);
        }

        // Flip sprite to face target
        if (horizontalDirection > 0 && spriteRenderer.flipX) // Moving right, but sprite is flipped left
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalDirection < 0 && !spriteRenderer.flipX) // Moving left but sprite is facing right
        {
            spriteRenderer.flipX = true;
        }
    }

    public virtual void ChaseTarget(Transform targetTrans) // Renamed parameter
    {
        if (targetTrans == null)
        {
            // Debug.LogWarning("Target Transform is null. Cannot chase target.", this);
            return;
        }

        // Important: Update target position constantly
        // This ensures the enemy chases the player's current position
        ChaseTarget(targetTrans.position);

    }

    // --- Reusable Patrol Logic Method (protected for derived classes to call) ---
    // Derived enemy classes can call this method from their own Move() override
    // if they want to implement patrol behavior.
    // ABSTRACTION: This method encapsulates the details of waypoint-based patrol.
    protected void ExecutePatrolMovement()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0 || !isAlive)
        {
            if (enemyRb != null) enemyRb.linearVelocity = Vector2.zero; // Stop movement if no waypoints are set or enemy is not alive
            return; // Exit if no waypoints are set or enemy is not alive
        }

        Vector2 targetWaypoint = patrolWaypoints[currentWaypointIndex].position;
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;

        // Only apply horizontal movement if grounded
        if (!isGrounded)
        {
            if (enemyRb != null) enemyRb.linearVelocity = new Vector2(0, enemyRb.linearVelocity.y);
            return;
        }

        // --- Movement Logic ---
        // Apply movement using Rigidbody2D.linearVelocity
        if (enemyRb != null)
        {
            enemyRb.linearVelocity = new Vector2(direction.x * moveSpeed, enemyRb.linearVelocity.y);
            // Clamp horizontal speed
            enemyRb.linearVelocity = new Vector2(
                Mathf.Clamp(enemyRb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
                enemyRb.linearVelocity.y
            );
        }
        else
        {
            transform.position += (Vector3)(new Vector2(direction.x, 0) * moveSpeed * Time.deltaTime);
        }

        // Flip sprite based on movement direction
        if (direction.x > 0 && spriteRenderer.flipX) // Moving right, but sprite is flipped left
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0 && !spriteRenderer.flipX) // Moving left, but sprite is facing right
        {
            spriteRenderer.flipX = true;
        }

        // --- Waypoint Reached Logic ---
        if (Vector2.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            // Debug.Log($"{gameObject.name} reached waypoint {currentWaypointIndex}");
            AdvanceToNextWaypoint();
        }

        // Ledge detection for patrolling enemies
        CheckForLedge(); // Ensure this is called during patrol
    }

    // --- Helper Method to Advance to the Next Waypoint ---
    protected void AdvanceToNextWaypoint()
    {
        if (patrolStyle == PatrolType.Loop)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length; // Loop back to the start
        }
        else if (patrolStyle == PatrolType.PingPong)
        {
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= patrolWaypoints.Length)
                {
                    currentWaypointIndex = patrolWaypoints.Length - 2; // go back one from the end
                    movingForward = false; // Change direction to backward
                    if (currentWaypointIndex < 0 && patrolWaypoints.Length > 0) currentWaypointIndex = 0; // Edge case for very short paths
                }
            }

            else // moving backward
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1; // go back one from the start
                    movingForward = true; // Change direction to forward
                    if (currentWaypointIndex >= patrolWaypoints.Length && patrolWaypoints.Length > 0) currentWaypointIndex = patrolWaypoints.Length - 1; // Edge case for very short paths
                }
            }
        }
    }

    // --- Ground & Ledge Detection ---
    // Abstraction: These Methods Encapsulate the details of physical checks ---
    protected void CheckIfGrounded()
    {
        if (enemyGroundCheckPoint == null) return;
        isGrounded = Physics2D.OverlapCircle(enemyGroundCheckPoint.position, enemyGroundCheckRadius, groundLayer);
        // Debug.DrawLine(enemyGroundCheckPoint.position, enemyGroundCheckPoint.position + Vector3.down * enemyGroundCheckRadius, isGrounded ? Color.green : color.red);
    }

    protected void CheckForLedge()
    {
        if (ledgeCheckPoint == null) return;

        // Determine the direction of the raycast based on which way the enemy is currently moving
        // Use spriteRenderer.flipX to determine current facing direction
        Vector2 raycastDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right; // If flipped, facing left

        RaycastHit2D hit = Physics2D.Raycast(ledgeCheckPoint.position, raycastDirection, ledgeCheckDistance, groundLayer);

        // Debug.DrawRay(ledgeCheckPoint.position, raycastDirection * ledgeCheckDistance, hit.collider == null ? Color.blue : Color.white);

        // If the raycast DOES NOT hit anything, it means there's no ground ahead
        // AND if the enemy is currently moving horizontally (to prevent turning if pushed)
        if (hit.collider == null && Mathf.Abs(enemyRb.linearVelocity.x) > 0.1f) // Check for actual horizontal movement
        {
            Debug.Log($"Ledge detected for {gameObject.name}! Turning around.", this);
            movingForward = !movingForward; // Reverse patrol direction
            //Flip sprite (handled by ChaseTarget/ExecutePatrolMovement)
        }
    }

    // --- AI State Management ---
    // ABSTRACTION: This method abstracts the decision-making process for enemy behavior.
    public virtual void CheckAIState() // Changed to virtual, not public if only called internally
    {
        // Ensure player target is available
        if (playerTargetTransform == null && GameManager.Instance != null)
        {
            playerTargetTransform = GameManager.Instance.PlayerTransform;
        }

        if (playerTargetTransform != null && isGrounded) // Only chase if grounded
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);

            if (distanceToPlayer < chaseRange) // Player is within chase range
            {
                ChaseTarget(playerTargetTransform); // Chase the player
            }
            else if (distanceToPlayer > loseChaseRange) // Player is outside lose chase range
            {
                ExecutePatrolMovement(); // Patrol if player is too far
            }
            else
            {
                // Player is between chaseRange and loseChaseRange, might continue previous behavior or idle
                // For simplicity, let's say it continues chasing if it was chasing, or patrols if it was patrolling
                // For now, it will just patrol if it's not in chase range.
                ExecutePatrolMovement(); // Default to patrol if not actively chasing
            }
        }
        else // No player target, or not grounded
        {
            ExecutePatrolMovement(); // Default to patrol
        }
    }

    // --- Overriding Die() from Character ---
    public override void Die()
    {
        // ABSTRACTION: This method handles the common death process for all enemies.
        Debug.Log($"{gameObject.name} is defeated! (Base Enemy Die)");

        // Add score to the game manager when the enemy dies
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue); // Use scoreValue
        }

        // Disable physics simulation and stop movement immediately
        if (enemyRb != null)
        {
            enemyRb.simulated = false;
            enemyRb.linearVelocity = Vector2.zero;
        }

        // Disable collider to prevent further interactions
        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // Disable sprite renderer to hide the enemy visually
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Disable any UI elements related to the enemy, such as health sliders
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }

        // Call the base class Die method to handle common death logic (like setting isAlive = false)
        base.Die();

        // Destroy the enemy game object after a short delay for animations/effects
        Destroy(gameObject, 0.5f);
    }

    // --- Optimization: Deactivate/Activate when off/on screen ---
    private void OnBecameInvisible()
    {
        Debug.Log($"{gameObject.name} became invisible.");
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        enabled = false; // Disable THIS SCRIPT to stop its Update/FixedUpdate/AI logic
        if (enemyRb != null)
        {
            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.Sleep(); // Put Rigidbody to sleep for performance
        }
        // Optional: Disable collider if you don't want off-screen enemies to interact
        // Collider2D enemyCollider = GetComponent<Collider2D>();
        // if (enemyCollider != null) enemyCollider.enabled = false;
    }

    private void OnBecameVisible()
    {
        Debug.Log($"{gameObject.name} became visible.");
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        enabled = true; // Enable THIS SCRIPT to resume its Update/FixedUpdate/AI logic
        if (enemyRb != null)
        {
            enemyRb.WakeUp(); // Wake up Rigidbody
                              // Reset velocity to zero to prevent unexpected movement from previous state
            enemyRb.linearVelocity = Vector2.zero;
        }
        // Optional: Enable collider
        // Collider2D enemyCollider = GetComponent<Collider2D>();
        // if (enemyCollider != null) enemyCollider.enabled = true;

        // Re-evaluate AI state immediately upon becoming visible
        // This ensures the enemy starts with the correct behavior (chase/patrol)
        CheckAIState();
    }
}
