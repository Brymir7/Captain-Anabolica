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

    private void Start()
    {
        var pistolAndBullet = weapons[(int)WeaponType.Pistol];
        var pistolInstance = Instantiate(pistolAndBullet.weaponPrefab, weaponPosition);
        pistol = pistolAndBullet.weaponPrefab.GetComponent<Pistol>();
        Assert.IsTrue(pistol);
        var pistolBullet = pistolAndBullet.projectilePrefab.GetComponent<PistolBullet>();
        Assert.IsTrue(pistolBullet);
        pistol.SetBulletPrefab(pistolAndBullet.projectilePrefab);
    }


    public void HoldWeapon()
    {
    }

    public void SwitchWeapon()
    {
        selectedWeapon++;

        if ((int)selectedWeapon >= System.Enum.GetValues(typeof(WeaponType)).Length)
        {
            selectedWeapon = WeaponType.Pistol;
        }
    }


    public void SwitchToWeapon(WeaponType wType)
    {
        selectedWeapon = wType;
    }

    public void StopHoldingWeapon()
    {
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