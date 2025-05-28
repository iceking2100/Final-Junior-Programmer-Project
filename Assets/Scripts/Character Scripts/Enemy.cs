using UnityEngine;

public class Enemy : Character
{
    int AttackDamage;
    int ScoreValue;
    private float health;

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
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
            GameManager.Instance.AddScore(ScoreValue);
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
        health = this.Health; // Use the base class property with the current instance

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
