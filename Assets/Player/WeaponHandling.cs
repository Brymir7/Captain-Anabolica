using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public enum WeaponType
{
    Pistol = 0,
    GrenadeLauncher = 1
}

[System.Serializable]
public class WeaponInfo
{
    public GameObject weaponPrefab;
    public GameObject projectilePrefab;
}

public class WeaponHandling : MonoBehaviour
{
    private bool isTransitioning;
    private bool isHolding;
    public List<WeaponInfo> weapons;
    public Transform weaponPosition;
    private Pistol pistol;
    public WeaponType selectedWeapon = WeaponType.Pistol;


    private Dictionary<WeaponType, GameObject> weaponInstances = new Dictionary<WeaponType, GameObject>();
    public Transform placeholderPosition;
    private GameObject currentWeapon;

    private void Start()
    {
        foreach (WeaponType weaponType in System.Enum.GetValues(typeof(WeaponType)))
        {
            InitializeWeapon(weaponType);
            weaponInstances[weaponType].transform.SetParent(weaponPosition);
            weaponInstances[weaponType].SetActive(false);
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    weaponInstances[weaponType].transform.localPosition = new Vector3(-0.06f, -0.241f, 1.419f);
                    weaponInstances[weaponType].transform.localScale = Vector3.one * 3.0f;
                    break;
                case WeaponType.GrenadeLauncher: // refactor into unity interface if necessary
                    weaponInstances[weaponType].transform.localPosition = new Vector3(0.19f, -0.02f, 1.73f);
                    weaponInstances[weaponType].transform.rotation = Quaternion.Euler(0, 179, 3);
                    weaponInstances[weaponType].transform.localScale = Vector3.one * 0.2691f;
                    break;
            }
        }

        SetWeaponInHand(selectedWeapon);
    }

    private void InitializeWeapon(WeaponType weaponType)
    {
        var weaponInfo = weapons[(int)weaponType];
        var weaponInstance = Instantiate(weaponInfo.weaponPrefab, weaponPosition.position, weaponPosition.rotation);
        weaponInstance.transform.localScale = Vector3.one * 3.0f;
        weaponInstances[weaponType] = weaponInstance;

        if (weaponType == WeaponType.Pistol)
        {
            pistol = weaponInstance.GetComponent<Pistol>();
            Assert.IsTrue(pistol);
            var pistolBullet = weaponInfo.projectilePrefab.GetComponent<PistolBullet>();
            Assert.IsTrue(pistolBullet);
            pistol.SetBulletPrefab(weaponInfo.projectilePrefab);
        }
    }

    private void SetWeaponInHand(WeaponType weaponType)
    {
        currentWeapon = weaponInstances[weaponType];
        currentWeapon.transform.SetParent(weaponPosition);
        currentWeapon.SetActive(true);
    }

    public void SwitchWeapon()
    {
        currentWeapon.transform.SetParent(placeholderPosition);
        currentWeapon.SetActive(false);

        selectedWeapon++;
        if ((int)selectedWeapon >= System.Enum.GetValues(typeof(WeaponType)).Length)
        {
            selectedWeapon = WeaponType.Pistol;
        }

        SetWeaponInHand(selectedWeapon);
    }

    public void SwitchToWeapon(WeaponType wType)
    {
        currentWeapon.transform.SetParent(placeholderPosition);
        currentWeapon.SetActive(false);

        selectedWeapon = wType;

        SetWeaponInHand(wType);
    }

    public void ShootWeapon(Camera playerCamera)
    {
        switch (selectedWeapon)
        {
            case WeaponType.Pistol:
                pistol.ShootWeapon(playerCamera);
                break;
        }
    }
}