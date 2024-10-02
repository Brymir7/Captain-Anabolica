using Player.Weapons;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        public delegate void EnemyDeathEventHandler(Enemy enemy);

        public event EnemyDeathEventHandler OnEnemyDeath;
        protected EnemyType EnemyType;
        protected int Health;
        protected float MoveSpeed;
        protected Vector3 Velocity;
        protected Transform Player;
        protected int xpOnKill;
        [SerializeField] protected GameObject onHitVFX;
        [SerializeField] protected GameObject onDeathVFX;
        protected GameObject XpOrbPrefab;

        public void SetXP(int xpValue, GameObject prefab)
        {
            xpOnKill = xpValue;
            XpOrbPrefab = prefab;
        }

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
                KillSelf();
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

        private void KillSelf()
        {
            OnDeathCallback();
            OnEnemyDeath?.Invoke(this);
            Destroy(gameObject);
        }

        protected virtual void OnDeathCallback()
        {
            if (onDeathVFX != null)
            {
                Instantiate(onDeathVFX, transform.position, Quaternion.identity);
            }

            int groundLayer = LayerMask.NameToLayer("Ground");
            int groundLayerMask = 1 << groundLayer;
            Vector3 spawnPosition = transform.position + Vector3.up * 2f;
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, 100f, groundLayerMask))
            {
                spawnPosition = hit.point;
            }
            else
            {
                spawnPosition = transform.position;
            }

            var xpOrb = Instantiate(XpOrbPrefab, spawnPosition, Quaternion.identity);
            var xpOrbScript = xpOrb.GetComponent<XpOrb>();
            xpOrbScript.xpAmount = xpOnKill;
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
}