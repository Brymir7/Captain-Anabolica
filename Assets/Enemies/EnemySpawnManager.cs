using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class EnemySpawnManager : MonoBehaviour
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

    private float nextSpawnTime;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Update()
    {
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        CleanupDestroyedEnemies();
    }

    private void SpawnEnemy()
    {
        EnemySpawnInfo spawnInfo = GetRandomEnemySpawnInfo();
        Vector3 spawnPosition = GetRandomSpawnPosition(spawnInfo);
        
        GameObject enemyObject = Instantiate(spawnInfo.enemyPrefab, spawnPosition, Quaternion.identity) ;
        Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
        Assert.IsTrue(enemyComponent != null);
        enemyComponent.Initialize(spawnInfo.enemyType);
        activeEnemies.Add(enemyObject);
    }

    private EnemySpawnInfo GetRandomEnemySpawnInfo()
    {
        float totalWeight = 0f;
        foreach (var info in enemySpawnInfos)
        {
            totalWeight += info.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var info in enemySpawnInfos)
        {
            currentWeight += info.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return info;
            }
        }

        return enemySpawnInfos[0];
    }

    private Vector3 GetRandomSpawnPosition(EnemySpawnInfo spawnInfo)
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(spawnInfo.minSpawnDistance, spawnInfo.maxSpawnDistance);

        Vector3 spawnPosition = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance;
        spawnPosition.y = 3f;
        return spawnPosition;
    }

    private void CleanupDestroyedEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }
}