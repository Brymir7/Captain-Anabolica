using UnityEngine;
using UnityEngine.Assertions;

namespace Player.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float cooldown = 1.0f;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected int baseDamage = 1;
        protected float LastShotTime = 0.0f;

        //public virtual void SetProjectilePrefab(GameObject prefab)
        //{
        //    this.projectilePrefab = prefab;
        //}

        protected virtual bool CanShoot()
        {
            return Time.time - LastShotTime > cooldown;
        }

        public void UpgradeDamage(int damage)
        {
            baseDamage += damage;
        }

        public virtual void UpgradeReloadSpeed(float multiplier)
        {
            Assert.IsTrue(multiplier < 1.0f, $"Multiplier {multiplier} is less than 1.0");
            cooldown *= multiplier;
        }

        public abstract void UpgradeSpecialAbility();

        public virtual void Shoot(Camera playerCamera)
        {
            if (CanShoot())
            {
                Vector3 direction = CalculateShootDirection(playerCamera);
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                PostInitiationCallback(projectile, direction);
                LastShotTime = Time.time;
            }
        }

        protected virtual Vector3 CalculateShootDirection(Camera playerCamera)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return (hit.point - transform.position).normalized;
            }
            else
            {
                return ray.direction;
            }
        }

        protected abstract void PostInitiationCallback(GameObject projectile, Vector3 direction);

        public float GetTimeTillNextShot()
        {
            return Mathf.Max(cooldown - Time.time - LastShotTime, 0.0f);
        }
    }
}