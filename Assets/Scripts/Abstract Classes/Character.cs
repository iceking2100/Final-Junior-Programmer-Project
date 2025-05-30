using UnityEngine;
using UnityEngine.UI;

// INHERITANCE
public abstract class Character : MonoBehaviour
{
    public float Health { get; protected set; } // ENCAPSULATION
    public int AttackDamage { get; protected set; } // ENCAPSULATION

    protected int ScoreValue; // ENCAPSULATION // This can be used to track score or points for the character
    protected float attackRange; // Default attack range, can be modified in derived classes
    private float AttackRange
    {
        get { return attackRange; }
        set { attackRange = Mathf.Max(0, value); } // Ensure attack range is non-negative
    }
    // INHERITANCE
    // This class serves as a base class for all characters in the game, including players and enemies.
    [Header("Character UI")]
    [SerializeField]
    protected Slider healthSlider; // Reference to the health slider UI element, can be used in derived classes // ENCAPSULATION

    public float moveSpeed;
    public bool isAlive;
    public float MaxHealth = 100f;
    protected void SetHealth(float value)
    {
        Health = Mathf.Clamp(value, 0, MaxHealth); // Ensure health does not exceed MaxHealth
        HealthUpdate();
    }
    public virtual void TakeDamage(int damageAmount)
    {
        SetHealth(Health - damageAmount); // or Health -= damageAmount
        HealthUpdate(); // Call the UI update method
    }

    public abstract void Attack(); // ABSTRACTION // POLYMORPHISM

    public abstract void Move(); // ABSTRACTION // POLYMORPHISM

    public virtual void HealthUpdate() // POLYMORPHISM
    {
        if (healthSlider != null)
        {
            healthSlider.value = Health; // Update the health slider value
        }
        else
        {
            Debug.LogWarning("Health Slider is not assigned!", this);
        }
    }

    public virtual void Die() // POLYMORPHISM
    {
        Debug.Log("Character Died.");
        isAlive = false;
        GameManager.Instance.AddScore(ScoreValue);
    }
    protected virtual void Awake()
    {
        SetHealth(MaxHealth); // Initialize health to MaxHealth
        isAlive = true; // Character starts alive
    }
}
