using System;
using UnityEngine;

public abstract class Enemy : Character
{
    public int AttackDamage;
    public int ScoreValue;

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
        // Handle logic when the enemy becomes invisible, if needed
        // For example, you might want to disable the enemy or remove it from the scene
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            if (enemyRb != null) enemyRb.linearVelocity = Vector2.zero; // Stop movement when invisible
        }
    }

    private void OnBecameVisible()
    {
        // Handle logic when the enemy becomes visible again
        // For example, you might want to reactivate the enemy or reset its state
        // Ensure the enemy is active when it becomes visible again
        // This is important to avoid issues with the enemy being inactive when it should be active
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);

            if (enemyRb != null) enemyRb.linearVelocity = Vector2.zero; // Reset velocity when it becomes visible
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

    public override void Die()
    {
        // Call the base class Die method to handle common death logic
        // like playing a death animation, dropping loot, etc.
        base.Die();
        // Important: Ensure to handle any additional logic specific to the enemy's death here
        // For example, you might want to play a death sound or spawn a death effect
        Destroy(gameObject, 0.5f); // Destroy the enemy game object after a short delay
    }
}
