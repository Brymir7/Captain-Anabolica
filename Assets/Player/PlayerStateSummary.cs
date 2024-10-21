using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Player;
using Player.Weapons;
using UnityEngine;

public class PlayerStateSummary : MonoBehaviour
{
    [SerializeField] private WeaponHandling weaponHandling;
    [SerializeField] private PlayerXp playerXp;
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

    public float GetXpProgress()
    {
        return playerXp.GetCurrentXp() / (float)playerXp.GetXpRequirementForNextLevel();
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public float[] GetWeaponCooldowns()
    {
        return _weaponCooldowns;
    }

    private void Awake()
    {
        _maxHealth = health;
        InitializeWeaponUnlockStatuses();
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

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy = other.GetComponentInParent<Enemy>();
            }

            health -= enemy.GetDamage();
        }
    }
}