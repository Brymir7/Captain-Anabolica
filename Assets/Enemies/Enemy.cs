using Player.Weapons;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        public delegate void EnemyDeathEventHandler(Enemy enemy);

        public int updateDirectionEveryFrames;
        public event EnemyDeathEventHandler OnEnemyDeath;
        protected int CurrFrame;
        protected EnemyType EnemyType;
        protected int Health;

        protected float MoveSpeed;
        protected Vector3 Velocity;
        protected Transform Player;

        [SerializeField] protected GameObject onHitVFX;
        [SerializeField] protected GameObject onDeathVFX;

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

        public virtual void SetVelToPlayerDir()
        {
        }

        public virtual void SetForwardVecToPlayer()
        {
            Vector3 direction = Player.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            Velocity = transform.forward * MoveSpeed;
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

        protected virtual void OnDirectionUpdate(Vector3 newDirection)
        {
        }

        public void FixedUpdate()
        {
            CurrFrame += 1;
            if (CurrFrame >= updateDirectionEveryFrames)
            {
                SetForwardVecToPlayer();
                OnDirectionUpdate(Velocity);
                CurrFrame = 0;
            }

            Move();
        }

        public Vector3 GetFirstGroundBelow()
        {
            Vector3 up = transform.position + Vector3.up * 5f;
            RaycastHit hit;
            int groundLayer = LayerMask.NameToLayer("Ground");
            int groundLayerMask = 1 << groundLayer;
            Vector3 ret;
            if (Physics.Raycast(up, Vector3.down, out hit, 100f, groundLayerMask))
            {
                ret = hit.point;
            }
            else
            {
                ret = transform.position;
            }

            return ret;
        }

        protected virtual void BulletOnTriggerEnterCallback(Collider other, ProjectileBase bullet)
        {
            TakeDamage(bullet.GetDamage());
            Instantiate(onHitVFX, other.transform.position, Quaternion.identity);
        }

        protected virtual void BulletOnCollisionEnterCallback(Collision collision, ProjectileBase bullet)
        {
            var obj = collision.gameObject;
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
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bullet"))
            {
                var bullet = other.GetComponent<ProjectileBase>();
                if (!bullet.IsEnabled()) return;
                bullet.DestroyProjectile();
                BulletOnTriggerEnterCallback(other, bullet);
            }
        }

        protected void OnCollisionEnter(Collision collision)
        {
            var obj = collision.gameObject;
            if (obj.CompareTag("Bullet"))
            {
                var bullet = obj.GetComponent<ProjectileBase>();
                if (!bullet.IsEnabled()) return;
                bullet.DestroyProjectile();
                BulletOnCollisionEnterCallback(collision, bullet);
            }
        }
    }
}