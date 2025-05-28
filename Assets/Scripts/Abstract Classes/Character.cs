using UnityEngine;

// INHERITANCE
public abstract class Character : MonoBehaviour
{
    public float Health { get; protected set; } // ENCAPSULATION

    public float moveSpeed;
    public bool isAlive;
    public float MaxHealth = 100f;
    protected void SetHealth(float value)
    {
        Health = Mathf.Clamp(value, 0, MaxHealth); // Ensure health does not exceed MaxHealth
    }
    public abstract void TakeDamage(int damageAmount); // ABSTRACTION // POLYMORPHISM

    public abstract void Attack(); // ABSTRACTION // POLYMORPHISM

    public abstract void Move(); // ABSTRACTION // POLYMORPHISM

    public virtual void Die() // POLYMORPHISM
    {
        Debug.Log("Character Died.");
        isAlive = false;
    }
    protected virtual void Awake()
    {
        SetHealth(MaxHealth); // Initialize health to MaxHealth
        isAlive = true; // Character starts alive
    }
}
