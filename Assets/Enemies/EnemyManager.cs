using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

        [SerializeField] private List<EnemySpawnInfo> enemySpawnInfos;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxEnemies = 50;

        private float _nextSpawnTime;
        public GameObject xpOrbPrefab;
        public float difficultyModifier = 1f;
        public List<Enemy> activeEnemies = new List<Enemy>();
        private readonly List<Transform> _enemyTransforms = new List<Transform>();
        private readonly List<GameObject> _activeEnemyObjects = new List<GameObject>();

        public List<Transform> GetTransforms()
        {
            return _enemyTransforms;
        }

        private void FixedUpdate()
        {
            if (Time.time >= _nextSpawnTime && activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + spawnInterval;
            }
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            int index = activeEnemies.IndexOf(enemy);
            if (index >= 0)
            {
                Destroy(_activeEnemyObjects[index]);
                activeEnemies.RemoveAt(index);
                _enemyTransforms.RemoveAt(index);
                _activeEnemyObjects.RemoveAt(index);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
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
                    worm.amountOfSegments = amountOfSegments;
                    enemyComponent.SetHealth(amountOfSegments);
                    enemyComponent.transform.localScale = Vector3.one * (worm.amountOfSegments * difficultyModifier);
                    break;
            }

            enemyComponent.transform.parent = transform;
            enemyComponent.OnEnemyDeath += HandleEnemyDeath;
            enemyComponent.SetXP(GetRandomXpValue(spawnInfo.enemyType), xpOrbPrefab);
            activeEnemies.Add(enemyComponent);
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