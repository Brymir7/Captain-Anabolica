using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> weaponPrefabs;
        [FormerlySerializedAs("_enemyManager")] [SerializeField] private EnemyManager enemyManager;

        public void OnPlayerLevelUp(int level)
        {
            CheckAndSpawnWeapons(level);
        }

        private void CheckAndSpawnWeapons(int level)
        {
            SpawnWeapon();
        }

        private void SpawnWeapon()
        {
            int randomIndex = Random.Range(0, weaponPrefabs.Count);
            enemyManager.SpawnWeapon(new EnemyManager.WeaponSpawnInfo
            {
                prefab = weaponPrefabs[randomIndex],
                SpawnPosition = null
            });
        }
    }
}