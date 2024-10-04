using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Player.Weapons
{
    [System.Serializable]
    public enum WeaponType
    {
        Pistol = 0,
        GrenadeLauncher = 1
    }

    [System.Serializable]
    public enum UpgradeType
    {
        Damage = 0,
        ReloadSpeed = 1,
        SpecialAbility = 2,
    }

    public class WeaponHandling : MonoBehaviour
    {
        private bool _isTransitioning;
        private bool _isHolding;
        [FormerlySerializedAs("weapons")] public List<GameObject> weaponsPrefabs;
        public Transform weaponPosition;
        private Pistol _pistol;
        private Launcher _launcher;
        private byte _hasUnlockedWeapons;
        public WeaponType selectedWeapon = WeaponType.Pistol;
        private Dictionary<WeaponType, GameObject> _weaponInstances = new Dictionary<WeaponType, GameObject>();
        private GameObject _currentWeapon;

        public void FillWeaponCooldowns(float[] cooldowns)
        {
            for (int i = 0; i < weaponsPrefabs.Count; i++)
            {
                WeaponType weaponType = (WeaponType)i;

                if (HasUnlockedWeapon(weaponType) == false)
                {
                    cooldowns[i] = -1f;
                    continue;
                }

                var weapon = _weaponInstances[weaponType].GetComponent<WeaponBase>();
                cooldowns[i] = weapon.GetTimeTillNextShot();
            }
        }

        public bool HasUnlockedWeapon(WeaponType weapon)
        {
            return (_hasUnlockedWeapons & (1 << (int)weapon)) > 0;
        }

        public void UnlockWeapon(WeaponType weapon)
        {
            Assert.IsTrue((int)weapon < 7);
            var newUnlock = (1 << (int)weapon) | _hasUnlockedWeapons;
            _hasUnlockedWeapons = (byte)newUnlock;
            print(_hasUnlockedWeapons);
        }

        public void UpgradeWeapon(WeaponType weapon, UpgradeType upgrade)
        {
            switch (weapon)
            {
                case WeaponType.Pistol:
                    switch (upgrade)
                    {
                        case UpgradeType.Damage:
                            _pistol.UpgradeDamage(1);
                            break;
                        case UpgradeType.ReloadSpeed:
                            _pistol.UpgradeReloadSpeed(0.9f);
                            break;
                        case UpgradeType.SpecialAbility:
                            _pistol.UpgradeSpecialAbility();
                            break;
                    }

                    break;
                case WeaponType.GrenadeLauncher: 
                    switch (upgrade)
                    {
                        case UpgradeType.Damage:
                            _launcher.UpgradeDamage(1);
                            break;
                        case UpgradeType.ReloadSpeed:
                            _launcher.UpgradeReloadSpeed(0.9f);
                            break;
                        case UpgradeType.SpecialAbility:
                            _launcher.UpgradeSpecialAbility();
                            break;
                    }

                    break;
            }
        }

        public void UpgradeWeaponRandomAttribute(WeaponType weapon)
        {
            var randomIndex = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(UpgradeType)).Length);
            UpgradeType upgrade = (UpgradeType)randomIndex;
            UpgradeWeapon(weapon, upgrade);
        }

        private void Start()
        {
            foreach (WeaponType weaponType in System.Enum.GetValues(typeof(WeaponType)))
            {
                InitializeWeapon(weaponType);
                _weaponInstances[weaponType].transform.SetParent(weaponPosition);
                _weaponInstances[weaponType].SetActive(false);
                switch (weaponType)
                {
                    case WeaponType.Pistol:
                        _weaponInstances[weaponType].transform.localPosition = new Vector3(-0.06f, -0.241f, 1.419f);
                        _weaponInstances[weaponType].transform.localScale = Vector3.one * 3.0f;
                        break;
                    case WeaponType.GrenadeLauncher: // refactor into unity interface if necessary
                        _weaponInstances[weaponType].transform.localPosition = new Vector3(0.19f, -0.02f, 1.73f);
                        _weaponInstances[weaponType].transform.rotation = Quaternion.Euler(0, 179, 3);
                        _weaponInstances[weaponType].transform.localScale = Vector3.one * 0.2691f;
                        break;
                }
            }

            SetWeaponInHand(selectedWeapon);
        }

        private void InitializeWeapon(WeaponType weaponType)
        {
            var weaponInfo = weaponsPrefabs[(int)weaponType];
            var weaponInstance = Instantiate(weaponInfo, weaponPosition.position, weaponPosition.rotation);
            weaponInstance.transform.localScale = Vector3.one * 3.0f;
            _weaponInstances[weaponType] = weaponInstance;

            if (weaponType == WeaponType.Pistol)
            {
                _pistol = weaponInstance.GetComponent<Pistol>();
                Assert.IsTrue(_pistol);
            }

            if (weaponType == WeaponType.GrenadeLauncher)
            {
                _launcher = weaponInstance.GetComponent<Launcher>();
                Assert.IsTrue(_launcher);
            }
        }

        private void SetWeaponInHand(WeaponType weaponType)
        {
            _currentWeapon = _weaponInstances[weaponType];
            _currentWeapon.transform.SetParent(weaponPosition);
            _currentWeapon.SetActive(true);
        }

        public void SwitchWeapon()
        {
            var newWINT = ((int)selectedWeapon + 1) % System.Enum.GetValues(typeof(WeaponType)).Length;
            var newW = (WeaponType)newWINT;
            if (!HasUnlockedWeapon(newW))
            {
                return;
            }

            _currentWeapon.SetActive(false);
            selectedWeapon = newW;
            SetWeaponInHand(selectedWeapon);
        }

        public void SwitchToWeapon(WeaponType wType)
        {
            if (!HasUnlockedWeapon(wType))
            {
                return;
            }

            _currentWeapon.SetActive(false);
            selectedWeapon = wType;
            SetWeaponInHand(wType);
        }

        public void ShootWeapon(Camera playerCamera)
        {
            switch (selectedWeapon)
            {
                case WeaponType.Pistol:
                    _pistol.Shoot(playerCamera);
                    break;
                case WeaponType.GrenadeLauncher:
                    _launcher.Shoot(playerCamera);
                    break;
            }
        }
    }
}