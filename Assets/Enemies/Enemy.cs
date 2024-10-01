using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyType EnemyType;
    protected int Health;
    protected float MoveSpeed;
    public Vector3 velocity;
    protected Transform Player;

    public virtual void Initialize(EnemyType type)
    {
        EnemyType = type;
        Health = 1;
        MoveSpeed = 0.05f;
        velocity = Vector3.zero;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetHealth(int newHealth)
    {
        this.Health = newHealth;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        MoveSpeed = newMoveSpeed;
    }

    public virtual void Move()
    {
        transform.position += velocity;
    }

    public virtual void TargetPlayer()
    {
        velocity = transform.forward * MoveSpeed;
    }

    public virtual void LookAtPlayer()
    {
        Vector3 direction = Player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public abstract void Attack();

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public delegate void EnemyDeathEventHandler(Enemy enemy);

    public event EnemyDeathEventHandler OnEnemyDeath;
    private bool _isDead = false;

    protected void Die()
    {
        if (!_isDead)
        {
            _isDead = true;
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
            ProjectileBase bullet = other.GetComponent<ProjectileBase>();
            TakeDamage(bullet.GetDamage());
            Destroy(other.gameObject);
        }
    }
}