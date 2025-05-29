using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    [SerializeField] private int jumpForce = 10;
    public bool HasKey { get; internal set; } = true;

    // Player-specific properties
    // Health, move speed, and other properties can be set in the inspector or initialized here
    [SerializeField] private float speedModifier = 300f; // Speed modifier for player movement
    [SerializeField] private float maxHorizontalSpeed = 10f; // Maximum horizontal speed for the player
    [SerializeField] private float playerHealthModifier = 2f; // Health modifier for the player
    [SerializeField] private Slider healthSlider; // Reference to the health slider UI element

    private Rigidbody2D playerRb;

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

        // Ensure groundCheckPoint is assigned
        if (groundCheckPoint == null)
        {
            Debug.LogError("GroundCheckPoint not assigned to Player Script!", this);
        }

        // --- Initialize Health Slider ---
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth; // Set the maximum value of the health slider
            healthSlider.value = Health; // Set the initial value of the health slider
        }
        else
        {
            Debug.LogWarning("Health Slider is not assigned in Player Script. Health UI will not be updated.", this);
        }
    }

    private void Update()
    {
        if (isAlive)
        {
            
            HandleJumpInput();
            // Add jump logic here if needed
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
        throw new System.NotImplementedException();
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
