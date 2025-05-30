using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    [SerializeField] private int jumpForce = 10;
    public bool HasKey { get; internal set; } = false;

    // Player-specific properties
    // Health, move speed, and other properties can be set in the inspector or initialized here
    [Header("Player Properties")] // New Header for player properties
    [SerializeField] private float speedModifier = 300f; // Speed modifier for player movement
    [SerializeField] private float maxHorizontalSpeed = 10f; // Maximum horizontal speed for the player
    [SerializeField] private float playerHealthModifier = 2f; // Health modifier for the player
    [SerializeField] private int playerAttackDamage = 20; // Player's attack damage
    [SerializeField] private float playerPunchAttackRange = 1.5f; // Player's attack range
    [SerializeField] private float playerAttackCooldownDuration = 1f; // Cooldown time for player attacks
    private float attackCooldownTimer;

    [Header("Player Rigidbody")] // New Header for Rigidbody properties
    private Rigidbody2D playerRb;

    [Header("Animation")] // New Header for animation properties
    [SerializeField] private Animator animator; // Reference to the Animator component (assigned in Inspector)

    // --- Raycast Grounded Detection Variables ---
    [Header("Grounded Detection")]
    [SerializeField] private Transform groundCheckPoint; // Transform to check for ground
    [SerializeField] private float groundCheckRadius = 0.1f; // Distance to check for ground
    [SerializeField] private LayerMask groundLayer; // Layer mask for ground detection

    private bool isGrounded; // Flag to check if the player is grounded

    // --- End Raycast Grounded Detection Variables ---

    protected override void Awake()
    {
        base.Awake(); // always call base.Awake() first to ensure base class initialization
        // Get Rigidbody2D component if not already done in Start
        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the Player GameObject.");
        }
        // Initialize player-specific properties

        MaxHealth = base.MaxHealth * playerHealthModifier; // Set health based on the player health modifier

        SetHealth(MaxHealth); // Initialize health to MaxHealth

        moveSpeed += speedModifier; // Increase move speed by the speed modifier

        AttackDamage = playerAttackDamage; // Set attack damage for the player

        attackCooldownTimer = 0f;

        // Ensure groundCheckPoint is assigned
        if (groundCheckPoint == null)
        {
            Debug.LogError("GroundCheckPoint not assigned to Player Script!", this);
        }

        // Register the player with the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(transform); // Register the player transform with the GameManager
            GameManager.Instance.RegisterPlayerObject(this); // Set the player instance in the GameManager
        }
        else
        {
            Debug.LogError("GameManager instance is not available. Player registration failed.", this);
        }

/*        // --- Initialize Health Slider ---
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth; // Set the maximum value of the health slider
            healthSlider.value = Health; // Set the initial value of the health slider
        }
        else
        {
            Debug.LogWarning("Health Slider is not assigned in Player Script. Health UI will not be updated.", this);
        }*/
    }

    private void Update()
    {
        if (!isAlive) return;
        
        HandleJumpInput();
            
        // Update attack cooldown timer
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

/*        // Animator parameters for movement
        if (animator != null)
        {
            animator.SetFloat("m_Speed", Mathf.Abs(playerRb.linearVelocity.x));
            animator.SetBool("isGrounded", isGrounded);
        }*/

        if (Input.GetButtonDown("Fire1")) // Check if the attack button is pressed
        {
           Attack(); // Call the attack method
        }
        
    }

    // --- Grounded Check Method ---
    private void FixedUpdate() // Use FixedUpdate for physics-related checks
    {
        Move();
        CheckIfGrounded();
    }

    private void CheckIfGrounded()
    {
        // Perform a raycast to check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        // Debug.DrawLine(groundCheckPoint.position - new Vector3(groundCheckRadius, 0, 0), groundCheckPoint.position + new Vector3(groundCheckRadius, 0, 0), Color.blue); // Visualize the ground check in the editor
    }

    // --- Handle Jump Input Method ---
    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // Check if jump button is pressed and player is grounded
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // Debug.Log("Player jumped!");
        }
    }

    public override void Attack()
    {
        // Check if the player is alive before attacking
        if (!isAlive)
        {
            Debug.LogWarning("Player is dead and cannot attack!");
            return; // Exit if the player is not alive
        }
        // Check if the attack is off cooldown
        if (attackCooldownTimer > 0)
        {
            Debug.Log("Player's attack is on cooldown!");
            return; // Exit if the attack is on cooldown
        }
        // Perform the attack logic
        // You can also play an attack animation or sound here if desired
        // For example, you can use an Animator component to trigger an attack animation

        if (animator != null)
        {
            animator.SetTrigger("m_Punch"); // Assuming you have an "Attack" trigger in your Animator
        }
        else
        {
            Debug.LogWarning("Animator component is not assigned in Player Script. Attack animation will not be played.", this);
        }



        // You can also implement a specific attack method here if needed
        // For example, you can check for nearby enemies and apply damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1f * playerPunchAttackRange, LayerMask.GetMask("Enemy")); // Adjust the radius and layer mask as needed
        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemy.attachedRigidbody.linearVelocity = Vector2.MoveTowards(enemy.attachedRigidbody.linearVelocity, -transform.position, 10.0f); // knockback effect
            // Check if the enemy has an Enemy script attached
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(AttackDamage); // Call the TakeDamage method on the enemy
                // Optionally, you can add logic to check if the enemy is defeated and handle it accordingly
                if (enemyScript.Health <= 0)
                {
                    // Optionally, you can add logic to handle enemy defeat, such as playing a death animation or sound
                    Debug.Log($"{enemy.name} has been defeated!");
                }
                // Log the attack for debugging purposes
                Debug.Log($"Player attacked {enemy.name} for {AttackDamage} damage.");
            }
        }

        // Reset the attack cooldown timer
        attackCooldownTimer = playerAttackCooldownDuration;




        // You can also play an attack sound effect here if desired
        // AudioSource audioSource = GetComponent<AudioSource>();
        // if (audioSource != null)
        // {
        //     audioSource.Play(); // Play the attack sound effect
        //     Debug.Log("Player attack sound effect played.");
        //  }
    }

    public override void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate desired horizontal velocity
        float targetXVelocity = horizontalInput * moveSpeed * Time.fixedDeltaTime; // Use Time.fixedDeltaTime for consistent physics calculations

        // Get the current velocity of the player
        Vector2 currentLinearVelocity = playerRb.linearVelocity; // use Rigidbody2D's linearVelocity for physics-based movement

        // Smoothly approach the desired horizontal velocity
        // you can use a smaller smoothing factor for crisper movement
        currentLinearVelocity.x = Mathf.Lerp(currentLinearVelocity.x, targetXVelocity, 0.1f);

        // Clamp the horizontal velocity to prevent excessive speed
        currentLinearVelocity.x = Mathf.Clamp(currentLinearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);

        // Apply the new velocity to the player
        playerRb.linearVelocity = currentLinearVelocity;

    }

    public override void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Remaining health: {Health}");
        if (Health <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
                Debug.Log("Player has died and lost a life. Remaining lives: " + GameManager.Instance.GetPlayerLives());
                if (GameManager.Instance.GetPlayerLives() > 0)
                {
                    GameManager.Instance.RespawnPlayer(); // Respawn player if lives are remaining
                    SetHealth(MaxHealth); // Reset health to max health
                }
                else
                {
                    Debug.Log("No more lives left. Game Over.");
                    Die(); // Call the Die method to handle game over logic
                }
            }
        }
    }

    public override void Die()
    {
        isAlive = false;
        playerRb.simulated = false; // Disable physics simulation on death
        Debug.Log("Player has died.");
        // Additional logic for player death can be added here, such as playing a death animation or sound
    }
}
