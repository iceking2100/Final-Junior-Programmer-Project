using UnityEngine;

public class Player : Character
{
    [SerializeField] private int jumpForce = 10;
    public bool HasKey { get; internal set; } = true;
    public float health; // Player's health
    // Player-specific properties
    // Health, move speed, and other properties can be set in the inspector or initialized here

    private Rigidbody2D playerRb;



    private void Start()
    {
        // Ensure the Rigidbody2D component is attached to the Player GameObject
        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the Player GameObject.");
        }
        // Initialize player properties

        health = this.Health; // Use the base class property with the current instance
        playerRb = GetComponent<Rigidbody2D>();
        moveSpeed = 5f; // Set initial move speed
        isAlive = true; // Player starts alive

        if (playerRb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the Player GameObject.");
        }
    }
    private void Update()
    {
        if (isAlive)
        {
            Move();
            // Add jump logic here if needed
        }
    }
    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        playerRb.AddForce(Vector2.left * horizontalInput * moveSpeed * Time.deltaTime, 0);

        if (Input.GetButtonDown("Jump") && playerRb.linearVelocity.y == 0) // Check if on ground
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

    }

    public override void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Remaining health: {health}");
        if (health <= 0)
        {
            GameManager.Instance.LoseLife();
            Debug.Log("Player has died and lost a life. Remaining lives: " + GameManager.Instance.playerLives);
            health = 100; // Reset health to 100 or any desired value
        }
        if (health <= 0 && GameManager.Instance.playerLives <= 0)
        {
            Die();
        }
    }
}
