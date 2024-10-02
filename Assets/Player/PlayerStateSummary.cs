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
    
    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    void Start()
    {
        _maxHealth = health;
    }


    void Update()
    {
        _weaponCooldowns = weaponHandling.GetWeaponCooldowns();
        print(_weaponCooldowns);
    }
}