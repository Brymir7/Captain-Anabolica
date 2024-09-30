using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float speed = 100f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float lifetime = 0f;
    [SerializeField] protected float maxDistance = 100f;

    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (rb.position.magnitude > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public virtual void SetDirection(Vector3 direction)
    {
        rb.velocity = direction.normalized * speed;
    }

    public virtual void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (rb != null && rb.velocity != Vector3.zero)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    public virtual void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public virtual void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
        CancelInvoke("DestroyProjectile");
        Invoke("DestroyProjectile", lifetime);
    }

    public virtual void SetMaxDistance(float newMaxDistance)
    {
        maxDistance = newMaxDistance;
    }

    protected virtual void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public virtual int GetDamage()
    {
        return damage;
    }
}