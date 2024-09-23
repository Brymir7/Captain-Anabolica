using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

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

    private float nextSpawnTime;
    public List<GameObject> activeEnemies = new List<GameObject>();
    private List<Transform> enemyTransforms = new List<Transform>();
    
    public List<Transform> GetTransforms()
    {
        return enemyTransforms;
    }

    private void FixedUpdate()
    {
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
        
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Enemy enemy = activeEnemies[i].GetComponent<Enemy>();
            Assert.IsTrue(enemy != null);
            if (enemy.IsDead())
            {
                Destroy(activeEnemies[i]);
                activeEnemies.RemoveAt(i);
                enemyTransforms.RemoveAt(i);
            }
        }
    }
    
    private void SpawnEnemy()
    {
        EnemySpawnInfo spawnInfo = GetRandomEnemySpawnInfo();
        Vector3 spawnPosition = GetRandomSpawnPosition(spawnInfo);
        
        GameObject enemyObject = Instantiate(spawnInfo.enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
        Assert.IsTrue(enemyComponent != null);
        enemyComponent.Initialize(spawnInfo.enemyType);
        activeEnemies.Add(enemyObject);
        enemyTransforms.Add(enemyObject.transform);
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


}