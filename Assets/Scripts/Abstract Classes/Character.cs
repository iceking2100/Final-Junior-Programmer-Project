using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public float Health 
    { get; private set; }
    
    public float moveSpeed;
    public bool isAlive;

    public abstract void TakeDamage(int damageAmount);

    public abstract void Attack();

    public abstract void Move();

    public virtual void Die()
    {
        Debug.Log("Character Died.");
        isAlive = false;
    }
}
