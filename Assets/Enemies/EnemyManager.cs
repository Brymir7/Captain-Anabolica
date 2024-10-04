using System.Collections.Generic;
using System.Linq;
using Player.Weapons;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawnInfo
        {
            public EnemyType enemyType;
            public GameObject enemyPrefab;
            public float spawnWeight = 1f;
            public float minSpawnDistance = 10f;
            public float maxSpawnDistance = 20f;
        }

        [System.Serializable]
        public class WeaponSpawnInfo
        {
            public GameObject prefab;
            public Vector3? SpawnPosition;
        }

        [SerializeField] private List<EnemySpawnInfo> enemySpawnInfos;
        [SerializeField] private float baseSpawnInterval;
        [SerializeField] private int baseMaxEnemies;
        private float _nextSpawnTime;
        [SerializeField] private GameObject xpOrbPrefab;
        [FormerlySerializedAs("_difficultyModifier")] [SerializeField] private float difficultyModifier = 1f;

        [FormerlySerializedAs("OneDeltaDifficultyPerXSeconds")] [SerializeField]
        private int oneDeltaDifficultyPerXSeconds;

        private List<Enemy> _activeEnemies = new List<Enemy>();
        private readonly List<Transform> _enemyTransforms = new List<Transform>();
        private readonly List<GameObject> _activeEnemyObjects = new List<GameObject>();
        private Queue<WeaponSpawnInfo> _weaponSpawnQueue = new Queue<WeaponSpawnInfo>();

        public List<Transform> GetTransforms()
        {
            return _enemyTransforms;
        }

        public void SpawnWeapon(WeaponSpawnInfo obj)
        {
            _weaponSpawnQueue.Enqueue(obj);
        }

        private void Update()
        {
            float currentSpawnInterval = baseSpawnInterval / difficultyModifier;
            float currentMaxEnemies = baseMaxEnemies * difficultyModifier;
            if (Time.time >= _nextSpawnTime && _activeEnemies.Count < currentMaxEnemies)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + currentSpawnInterval;
            }

            difficultyModifier += Time.deltaTime / oneDeltaDifficultyPerXSeconds;
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            int index = _activeEnemies.IndexOf(enemy);
            if (index >= 0)
            {
                var xpOrb = Instantiate(xpOrbPrefab, enemy.GetFirstGroundBelow(), Quaternion.identity);
                var xpOrbScript = xpOrb.GetComponent<XpOrb>();
                xpOrbScript.xpAmount = 20; // TODO
                if (_weaponSpawnQueue.Count > 0)
                {
                    var weapon = _weaponSpawnQueue.Dequeue();

                    Instantiate(weapon.prefab,
                        weapon.SpawnPosition.HasValue ? weapon.SpawnPosition.Value : enemy.GetFirstGroundBelow() + weapon.prefab.transform.localScale,
                        Quaternion.identity);
                }

                Destroy(_activeEnemyObjects[index]);
                _activeEnemies.RemoveAt(index);
                _enemyTransforms.RemoveAt(index);
                _activeEnemyObjects.RemoveAt(index);
            }
        }

        private void SpawnEnemy()
        {
            EnemySpawnInfo spawnInfo = GetRandomEnemySpawnInfo();
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnInfo);

            GameObject enemyObject = Instantiate(spawnInfo.enemyPrefab, spawnPosition, Quaternion.identity);
            Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
            Assert.IsTrue(enemyComponent);
            enemyComponent.Initialize(spawnInfo.enemyType);
            switch (spawnInfo.enemyType)
            {
                case EnemyType.Worm:
                    var worm = enemyObject.GetComponent<WormEnemy>();
                    var amountOfSegments = Random.Range(5, 12);
                    Assert.IsTrue(amountOfSegments < 12,
                        "IF AMOUNT OF SEGMENTS > 12 TARGETTING OF WORM BREAKS; DUE TO IT NOT CORRECTLY ADJUSTING BASED ON WORM TILE HEIGHT");
                    worm.amountOfSegments = amountOfSegments;
                    enemyComponent.SetHealth(amountOfSegments);
                    enemyComponent.transform.localScale = Vector3.one * worm.amountOfSegments;
                    break;
                case EnemyType.Spider:
                    var spider = enemyComponent.GetComponent<SpiderEnemy>();
                    spider.SetMoveSpeed(0.09f);
                    spider.SetHealth(1);
                    break;
                case EnemyType.Skeleton:
                    var skeleton = enemyComponent.GetComponent<SkeletonEnemy>();
                    skeleton.SetMoveSpeed(0.06f);
                    skeleton.SetHealth(3);
                    break;
            }

            enemyComponent.transform.parent = transform;
            enemyComponent.OnEnemyDeath += HandleEnemyDeath;
            _activeEnemies.Add(enemyComponent);
            _enemyTransforms.Add(enemyObject.transform);
            _activeEnemyObjects.Add(enemyObject);
        }

        private EnemySpawnInfo GetRandomEnemySpawnInfo()
        {
            float totalWeight = enemySpawnInfos.Sum(info => info.spawnWeight);
            float randomValue = Random.Range(0f, totalWeight);
            return enemySpawnInfos.FirstOrDefault(info => (randomValue -= info.spawnWeight) < 0);
        }

        private static int GetRandomXpValue(EnemyType eType)
        {
            switch (eType)
            {
                case EnemyType.Skeleton:
                    return Random.Range(8, 15);
                case EnemyType.Spider:
                    return Random.Range(10, 15);
                case EnemyType.Worm:
                    return Random.Range(25, 30);
                default:
                    Debug.Log("Invalid E type for XP Value");
                    return 10;
            }
        }

        private Vector3 GetRandomSpawnPosition(EnemySpawnInfo spawnInfo)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(spawnInfo.minSpawnDistance, spawnInfo.maxSpawnDistance);

            Vector3 spawnPosition = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance;
            switch (spawnInfo.enemyType)
            {
                case EnemyType.Worm:
                    spawnPosition.y = 2f;
                    break;
                case EnemyType.Skeleton:
                    spawnPosition.y = 0.7f;
                    break;
                case EnemyType.Spider:
                    spawnPosition.y = 2f;
                    break;
            }

            return spawnPosition;
        }
    }
}