using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyType enemyType;
    protected int health;
    protected float moveSpeed;
    public Vector3 velocity;
    protected Transform player;

    public virtual void Initialize(EnemyType type)
    {
        enemyType = type;
        health = 1;
        moveSpeed = 0.05f;
        velocity = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetHealth(int newHealth)
    {
        this.health = newHealth;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public virtual void Move()
    {
        transform.position += velocity;
    }

    public virtual void TargetPlayer()
    {
        velocity = transform.forward * moveSpeed;
    }

    public abstract void LookAtPlayer();


    public abstract void Attack();

    public virtual void TakeDamage(int damage)
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

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            PistolBullet bullet = other.GetComponent<PistolBullet>();
            TakeDamage(bullet.GetDamage());
            Destroy(other.gameObject);
        }
    }
}