using UnityEngine;

namespace Player.Weapons
{
    public class Launcher : WeaponBase
    {
        protected int AmountOfRecursiveChildren = 0;
        protected int ChildrenPerRecursion = 0;
        protected int TotalAbilityUpgrades = 0;

        protected override void PostInitiationCallback(GameObject projectile, Vector3 direction)
        {
            var bulletComponent = projectile.GetComponent<LauncherBullet>();
            bulletComponent.SetDirection(direction);
            bulletComponent.SetDamage(baseDamage);
            bulletComponent.AddRecursiveChildren(AmountOfRecursiveChildren);
            bulletComponent.AddChildrenToRecursion(ChildrenPerRecursion);
        }

        public override void UpgradeSpecialAbility()
        {
            if (TotalAbilityUpgrades % 2 == 0)
            {
                ChildrenPerRecursion++;
            }
            else
            {
                AmountOfRecursiveChildren++;
            }
            TotalAbilityUpgrades++;
        }
    }
}