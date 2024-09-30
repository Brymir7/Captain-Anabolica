using UnityEngine;
public class Pistol : WeaponBase
{ 
    protected override void InitializeProjectile(GameObject projectile, Vector3 direction)
    {
        var bulletComponent = projectile.GetComponent<PistolBullet>();
        bulletComponent.SetDirection(direction);
    }
}