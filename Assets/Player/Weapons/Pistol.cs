﻿using UnityEngine;

namespace Player.Weapons
{
    public class Pistol : WeaponBase
    {
        [SerializeField] private GameObject onShootVFX;

        protected override void PostInitiationCallback(GameObject projectile, Vector3 direction)
        {
            var bulletComponent = projectile.GetComponent<PistolBullet>();
            bulletComponent.SetDirection(direction);
            bulletComponent.SetDamage(baseDamage);
            Instantiate(onShootVFX, transform.position + transform.forward * 0.2f, transform.rotation);
        }

        public override void UpgradeSpecialAbility()
        {
            print("didnt impelemnt to upgrade");
        }
    }
}