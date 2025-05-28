using UnityEngine;

public class Enemy : Character
{
    public int AttackDamage;
    public int ScoreValue;

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

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

    public virtual void ChaseTarget(Vector2 targetPosition)
    {
        // Implement logic to chase the target position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    public virtual void ChaseTarget(Transform targetTransform)
    {
        // Implement logic to chase the target transform
        ChaseTarget(targetTransform.position);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = this.Health; // Use the base class property with the current instance

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameInvisible()
    {
        // Handle logic when the enemy becomes invisible, if needed
        // For example, you might want to disable the enemy or remove it from the scene
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnBecameVisible()
    {
        // Handle logic when the enemy becomes visible again, if needed
        // For example, you might want to re-enable the enemy or reset its state
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
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
