using System;
using UnityEngine;

public abstract class Enemy : Character
{
    // INHERITANCE: Enemy is a subclass of Character, which is an abstract class
    // This class represents a base class for all enemies in the game, inheriting from Character
    // ENCAPSULATION: Health, AttackDamage, and other properties are encapsulated within the class
    [Header("Enemy Settings")]
    [SerializeField]
    protected int scoreValue = 100; // Score value for defeating this enemy, can be set in the inspector
    protected SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component for visual effects (protected for derived class access)
    protected Transform targetTransform; // Reference to the target Transform (e.g., player) for chasing behavior
    protected Vector2 targetPosition;

    // --- Patrol specific variables (protected for derived class access) ---
    [Header("Patrol Settings")]
    [SerializeField]
    protected Transform[] patrolWaypoints; // Array to hold multiple waypoints for the patrol path
    protected int currentWaypointIndex = 0;
    protected bool movingForward = true; // For ping-pong patrol type

    // Reference to Rigidbody2D for physics-based movement (protected for derived class access)
    protected Rigidbody2D enemyRb;
    public enum PatrolType { Loop, PingPong }
    [SerializeField]
    protected PatrolType patrolStyle = PatrolType.Loop; // Default patrol style

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
        if (GameManager.Instance != null && GameManager.Instance.PlayerTransform != null)
        {
            targetTransform = GameManager.Instance.PlayerTransform; // Get the player's transform from the GameManager
            targetPosition = targetTransform.position; // Set initial target position to player's position
        }
        else
        {
            Debug.LogWarning("GameManager or PlayerTransform is not set. Enemy will not chase the player.", this);
        }

        // Validate waypoints ONLY IF patrolwaypoints is not null and length > 0
        // Specific enemies might not use patrol so don't disable if it's not assigned
        if (patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            // Initialize currentWaypointIndex for patrol
            currentWaypointIndex = 0;
        }
    }

    public abstract override void Attack();

    public abstract override void Move();

    public override void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Die();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(ScoreValue);
            }
            
        }
    }

    public override void HealthUpdate()
    {
        // Update the health slider UI if assigned
        if (healthSlider != null)
        {
            healthSlider.value = Health; // Update the health slider value
        }
        else
        {
            Debug.LogWarning("Health Slider is not assigned!", this);
        }
    }

    // --- Polymorphism: Overloaded ChaseTarget Methods ---
    // These methods allow different ways to chase a target, either by position or by Transform.
    public virtual void ChaseTarget(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        if (enemyRb != null)
        {
            enemyRb.linearVelocity = direction * moveSpeed; // using inherited moveSpeed from Character class
        }
        else
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }

    public virtual void ChaseTarget(Transform targetTransform)
    {
        
        // Chase the target using its Transform
        if (targetTransform == null)
        {
            Debug.LogWarning("Target Transform is null. Cannot chase target.", this);
            return; // Exit if targetTransform is null
        }

        if (targetTransform != null)
        {
            ChaseTarget(targetTransform.position);
        }
    }

    // --- Reusable Patrol Logic Method (protected for derived classes to call) ---
    // Derived enemy classes can call this method from thier own Move() override
    // if they want to implement patrol behavior.
    protected void ExecutePatrolMovement()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0 || !isAlive)
        {
            if (enemyRb != null) enemyRb.linearVelocity = Vector2.zero; // Stop movement if no waypoints are set or enemy is not alive
            return; // Exit if no waypoints are set or enemy is not alive
        }

        Vector2 targetWaypoint = patrolWaypoints[currentWaypointIndex].position;
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;

        // --- Movement Logic ---
        if (enemyRb != null)
        {
            enemyRb.linearVelocity = direction * moveSpeed; // using inherited moveSpeed from Character class
        }
        else
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }

        // --- Waypoint Reached Logic ---
        if (Vector2.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            Debug.Log($"{gameObject.name} reached waypoint {currentWaypointIndex}");
            AdvanceToNextWaypoint();
        }
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
                    if (currentWaypointIndex < 0) currentWaypointIndex = 0; // Ensure we don't go below zero 
                }
            }
        }
        else // moving backward
        {
            currentWaypointIndex--;
            if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 1; // go back one from the start
                movingForward = true; // Change direction to forward
                if (currentWaypointIndex >= patrolWaypoints.Length) currentWaypointIndex = patrolWaypoints.Length - 1; // Ensure we don't go beyond the last waypoint
            }
        }

    }

    // --- Optimization: Deactivate/Activate when off/on screen ---
    private void OnBecameInvisible()
    {
        // This method is called when the enemy becomes invisible to the camera
        // Handle logic when the enemy becomes invisible, if needed
        // For example, you might want to disable the enemy or remove it from the scene
        // Ensure the enemy is inactive when it becomes invisible
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Disable the sprite renderer to hide the enemy visually
        }
        // Optionally, you can also disable the enemy's Rigidbody2D to stop its movement
        if (enemyRb != null)
        {
            enemyRb.simulated = false; // Disable physics simulation when invisible
            enemyRb.linearVelocity = Vector2.zero; // Stop movement when invisible
        }
    }

    private void OnBecameVisible()
    {
        // Handle logic when the enemy becomes visible again
        // For example, you might want to reactivate the enemy or reset its state
        // Ensure the enemy is active when it becomes visible again
        // This is important to avoid issues with the enemy being inactive when it should be active

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // Enable the sprite renderer to show the enemy visually
        }
        if (enemyRb != null)
        {
            enemyRb.simulated = true; // Re-enable physics simulation when visible
            enemyRb.linearVelocity = Vector2.zero; // Reset velocity when it becomes visible
            // Reinitialize patrol if needed
            if (patrolWaypoints != null && patrolWaypoints.Length > 0)
            {
                currentWaypointIndex = 0; // Reset to the first waypoint when it becomes visible
            }
            // Reavaluate AI state if needed
            // For example, you might want to check if the enemy should chase the player or patrol
            // This can be done by calling a method to check the AI state or by setting a flag
            // Example: Check if the enemy should chase the player or patrol
            // CheckAIState(); // This method can be implemented to evaluate the AI state based on the game logic
        }
    }

    public virtual void CheckAIState()
    {
        // This method can be overridden in derived classes to implement specific AI state checks
        // For example, you might want to check if the enemy should chase the player or patrol
        // This can be done by checking the distance to the player or other game conditions
        // Example: If the player is within a certain range, chase them; otherwise, patrol
        if (currentWaypointIndex < 0)
        {
            Debug.LogWarning("Current waypoint index is negative. Resetting to 0.", this);
            currentWaypointIndex = 0; // Reset to the first waypoint if the index is negative
        }
        if (patrolWaypoints == null || patrolWaypoints.Length == 0)
        {
            Debug.LogWarning("Patrol waypoints are not set. Cannot check AI state.", this);
            return; // Exit if no waypoints are set
        }
        // Check if the player is within a certain range to chase them
        if (currentWaypointIndex == 0) {
            Debug.LogWarning("Current waypoint index is 0. Cannot chase player.", this);
            return; // Exit if the current waypoint index is 0, as it might not be valid for chasing
        }
        targetTransform = GameManager.Instance.PlayerTransform; // Get the player's transform from the GameManager

        if (GameManager.Instance != null && GameManager.Instance.PlayerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPosition);
            if (distanceToPlayer < 5f) // Example distance threshold for chasing
            {
                ChaseTarget(targetTransform); // Chase the player if within range
            }
            else
            {
                ExecutePatrolMovement(); // Patrol if the player is not within range
            }
        }
    }

    public override void Die()
    {
        // Call the base class Die method to handle common death logic
        // like playing a death animation, dropping loot, etc.
        base.Die();
        // Set isAlive to false to prevent further actions  
        isAlive = false;
        // Disable the enemy's Rigidbody2D to stop its movement
        if (enemyRb != null)
        {
            enemyRb.simulated = false; // Disable physics simulation on death
            enemyRb.velocity = Vector2.zero; // Stop movement immediately
        }
        // Optionally, you can also disable the enemy's collider to prevent further interactions
        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null) {
            enemyCollider.enabled = false; // Disable the collider to prevent further interactions
        }
        // Optionally, you can also disable the sprite renderer to hide the enemy visually
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Disable the sprite renderer to hide the enemy visually
        }

        // Disable any UI elements related to the enemy, such as health sliders
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false); // Hide the health slider UI
        }

        // Optionally, you can also play a death animation or sound effect here
        // For example, you might want to play a death animation or sound effect
        // Animator enemyAnimator = GetComponent<Animator>();
        // if (enemyAnimator != null)
        // {
        //     enemyAnimator.SetTrigger("Die"); // Assuming you have a "Die" trigger in your Animator
        // }
        // Debug.Log($"{gameObject.name} has died and will be destroyed.");
        // Optionally, you can also add a delay before destroying the enemy game object
        // This allows for any death animations or effects to play out before the enemy is removed from the scene
        // Important: Ensure to handle any additional logic specific to the enemy's death here
        // For example, you might want to play a death sound or spawn a death effect
        Destroy(gameObject, 0.5f); // Destroy the enemy game object after a short delay
    }
}
