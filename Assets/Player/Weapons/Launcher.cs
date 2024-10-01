using UnityEngine;

public class Launcher : WeaponBase
{
    protected override void InitializeProjectile(GameObject projectile, Vector3 direction)
    {
        var bulletComponent = projectile.GetComponent<LauncherBullet>();
        bulletComponent.SetDirection(direction);
    }
}