using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected float cooldown = 1.0f;
    [SerializeField] protected GameObject projectilePrefab;

    protected float LastShotTime = 0.0f;

    //public virtual void SetProjectilePrefab(GameObject prefab)
    //{
    //    this.projectilePrefab = prefab;
    //}

    protected virtual bool CanShoot()
    {
        return Time.time - LastShotTime > cooldown;
    }

    public virtual void Shoot(Camera playerCamera)
    {
        if (CanShoot())
        {
            Vector3 direction = CalculateShootDirection(playerCamera);
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            InitializeProjectile(projectile, direction);
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

    protected abstract void InitializeProjectile(GameObject projectile, Vector3 direction);
}