using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Player.Weapons
{
    [System.Serializable]
    public enum WeaponType
    {
        Pistol = 0,
        GrenadeLauncher = 1
    }


    public class WeaponHandling : MonoBehaviour
    {
        private bool _isTransitioning;
        private bool _isHolding;
        public List<GameObject> weapons;
        public Transform weaponPosition;
        private Pistol _pistol;
        private Launcher _launcher;
        private byte _hasUnlockedWeapons;
        public WeaponType selectedWeapon = WeaponType.Pistol;
        private Dictionary<WeaponType, GameObject> _weaponInstances = new Dictionary<WeaponType, GameObject>();
        private GameObject _currentWeapon;

        private bool HasUnlockedWeapon(WeaponType weapon)
        {
            return (_hasUnlockedWeapons & (1 << (int)weapon)) != 0;
        }

        public void UnlockWeapon(WeaponType weapon)
        {
            Assert.IsTrue((int)weapon < 7);
            var newUnlock = (1 << (int)weapon) | _hasUnlockedWeapons;
            _hasUnlockedWeapons = (byte)newUnlock;
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
            var weaponInfo = weapons[(int)weaponType];
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
            _currentWeapon.SetActive(false);

            selectedWeapon++;
            if ((int)selectedWeapon >= System.Enum.GetValues(typeof(WeaponType)).Length)
            {
                selectedWeapon = WeaponType.Pistol;
            }

            SetWeaponInHand(selectedWeapon);
        }

        public void SwitchToWeapon(WeaponType wType)
        {
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