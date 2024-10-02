using UnityEngine;

namespace Player.Weapons
{
    public class Launcher : WeaponBase
    {
        protected override void PostInitiationCallback(GameObject projectile, Vector3 direction)
        {
            var bulletComponent = projectile.GetComponent<LauncherBullet>();
            bulletComponent.SetDirection(direction); 
        }
    }
}