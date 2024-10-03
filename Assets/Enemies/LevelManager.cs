using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> weaponPrefabs;
    [SerializeField] private List<int> levelsToSpawnWeapons;
    [SerializeField] private EnemyManager _enemyManager;

    public void OnPlayerLevelUp(int level)
    {
        CheckAndSpawnWeapons(level);
    }

    private void CheckAndSpawnWeapons(int level)
    {
        if (levelsToSpawnWeapons.Contains(level))
        {
            SpawnWeapon();
        }
    }

    private void SpawnWeapon()
    {
        int randomIndex = Random.Range(0, weaponPrefabs.Count);
        _enemyManager.SpawnWeapon(new EnemyManager.WeaponSpawnInfo
        {
            prefab = weaponPrefabs[randomIndex],
            SpawnPosition = null
        });
    }
}