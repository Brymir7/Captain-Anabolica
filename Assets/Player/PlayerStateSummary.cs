using System;
using System.Collections;
using System.Collections.Generic;
using Player.Weapons;
using UnityEngine;

public class PlayerStateSummary : MonoBehaviour
{
    [SerializeField] private WeaponHandling weaponHandling;
    [SerializeField] private int health;
    private int _maxHealth;
    private float[] _weaponCooldowns = new float[8];

    [Serializable]
    private class WeaponUnlockStatus
    {
        public WeaponType weaponType;
        public bool isUnlocked;
    }

    [SerializeField] private WeaponUnlockStatus[] weaponUnlockStatuses;

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public float[] GetWeaponCooldowns()
    {
        return _weaponCooldowns;
}

    void Start()
    {
        _maxHealth = health;
        InitializeWeaponUnlockStatuses();

    }

    void Update()
    {
        weaponHandling.FillWeaponCooldowns(_weaponCooldowns);
        print(_weaponCooldowns[0]);
    }

    private void InitializeWeaponUnlockStatuses()
    {
        foreach (var status in weaponUnlockStatuses)
        {
            if (status.isUnlocked)
            {
                weaponHandling.UnlockWeapon(status.weaponType);
            }
        }
    }

}