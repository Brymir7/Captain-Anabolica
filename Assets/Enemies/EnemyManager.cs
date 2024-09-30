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
    public float difficultyModifier = 1f;
    public List<Enemy> activeEnemies = new List<Enemy>();
    private List<Transform> enemyTransforms = new List<Transform>();
    private List<GameObject> activeEnemyObjects = new List<GameObject>();

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
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        int index = activeEnemies.IndexOf(enemy);
        if (index >= 0)
        {
            Destroy(activeEnemyObjects[index]);
            activeEnemies.RemoveAt(index);
            enemyTransforms.RemoveAt(index);
            activeEnemyObjects.RemoveAt(index);
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
        switch (spawnInfo.enemyType)
        {
            case EnemyType.Worm:
                WormEnemy worm = enemyObject.GetComponent<WormEnemy>();
                var amountOfSegments = Random.Range(5, 12);
                worm.amountOfSegments = amountOfSegments;
                enemyComponent.SetHealth(amountOfSegments);
                enemyComponent.transform.localScale = Vector3.one * (worm.amountOfSegments * difficultyModifier);
                break;
            default:
                break;
        }

        enemyComponent.OnEnemyDeath += HandleEnemyDeath;
        activeEnemies.Add(enemyComponent);
        enemyTransforms.Add(enemyObject.transform);
        activeEnemyObjects.Add(enemyObject);
    }

    private EnemySpawnInfo GetRandomEnemySpawnInfo()
    {
        float totalWeight = enemySpawnInfos.Sum(info => info.spawnWeight);
        float randomValue = Random.Range(0f, totalWeight);
        return enemySpawnInfos.FirstOrDefault(info => (randomValue -= info.spawnWeight) < 0);
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