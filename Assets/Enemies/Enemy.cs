using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyType EnemyType;
    protected int Health;
    protected float MoveSpeed;
    protected Vector3 Velocity;
    protected Transform Player;
    [SerializeField] protected GameObject onHitVFX;

    public virtual void Initialize(EnemyType type)
    {
        EnemyType = type;
        Health = 1;
        MoveSpeed = 0.05f;
        Velocity = Vector3.zero;
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
        transform.position += Velocity;
    }

    public virtual void TargetPlayer()
    {
        Velocity = transform.forward * MoveSpeed;
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

    protected virtual void BulletOnTriggerEnterCallback(Collider other)
    {
        ProjectileBase bullet = other.GetComponent<ProjectileBase>();
        TakeDamage(bullet.GetDamage());
        Instantiate(onHitVFX, other.transform.position, Quaternion.identity);
    }

    protected virtual void BulletOnCollisionEnterCallback(Collision collision)
    {
        var obj = collision.gameObject;
        ProjectileBase bullet = obj.GetComponent<ProjectileBase>();
        TakeDamage(bullet.GetDamage());
        Instantiate(onHitVFX, obj.transform.position, Quaternion.identity);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            BulletOnTriggerEnterCallback(other);
            Destroy(other.gameObject);
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Bullet"))
        {
            BulletOnCollisionEnterCallback(collision);
            Destroy(obj);
        }
    }
}