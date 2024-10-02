using UnityEngine;

namespace Player.Weapons
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected int damage;
        [SerializeField] protected float lifetime;
        [SerializeField] protected float maxDistance;
        private bool _isEnabled = true;
        protected Rigidbody Rb;
        protected Collider Collid;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            if (Rb == null)
            {
                Rb = gameObject.AddComponent<Rigidbody>();
            }

            Collid = GetComponent<Collider>();
        }

        public bool IsEnabled()
        {
            return _isEnabled;
        }

        protected virtual void FixedUpdate()
        {
            if (Rb.position.magnitude > maxDistance)
            {
                Destroy(gameObject);
            }
        }

        public virtual void SetDirection(Vector3 direction)
        {
            Rb.velocity = direction.normalized * speed;
        }

        public virtual void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
            if (Rb != null && Rb.velocity != Vector3.zero)
            {
                Rb.velocity = Rb.velocity.normalized * speed;
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

        public void DestroyProjectile()
        {
            _isEnabled = false;
            Destroy(gameObject);
        }

        public void DisableProjectile()
        {
            _isEnabled = false;
        }

        public void EnableProjectile()
        {
            _isEnabled = true;
        }

        public int GetDamage()
        {
            return damage;
        }
    }
}