using System;
using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    protected EnemyType enemyType;
    protected float health;
    protected float moveSpeed;
    public Vector3 velocity;
    private Transform player;
    public virtual void Initialize(EnemyType type)
    {
        enemyType = type;
        health = 1f;
        moveSpeed = 0.05f;
        velocity = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void Move()
    {
        transform.position += velocity;
    }

    public virtual void TargetPlayer()
    {
        velocity = (player.position - transform.position).normalized * moveSpeed;
        velocity.y = 0f;
    }
    public abstract void LookAtPlayer(); 
    public abstract void Attack();
    
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    public delegate void EnemyDeathEventHandler(Enemy enemy);
    public event EnemyDeathEventHandler OnEnemyDeath;
    private bool isDead = false;

    protected void Die()
    {
        if (!isDead)
        {
            isDead = true;
            OnEnemyDeath?.Invoke(this); 
        }
    }
    
    public void FixedUpdate()
    {
        TargetPlayer();
        LookAtPlayer();
        Move();
    }
}