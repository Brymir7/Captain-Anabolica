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
        health = 100f;
        moveSpeed = 5f;
        velocity = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public abstract void Move();

    public virtual void TargetPlayer()
    {
        velocity = (player.position - transform.position).normalized * moveSpeed;
    }
    public abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        TargetPlayer();
        Move();
    }
}