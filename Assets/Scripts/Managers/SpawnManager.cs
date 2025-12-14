using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum SpawnType
    {
        Enemy,
        Collectible
    }

    [Header("Spawn Type")]
    public SpawnType spawnType;

    [Header("Prefabs")]
    public GameObject[] enemyPrefabs;
    public GameObject[] collectiblePrefabs;

    [Header("Spawners")]
    public Spawner[] spawners;

    [Header("Difficulty")]
    public float baseSpawnTime = 3f;
    public float minSpawnTime = 0.25f;
    public float difficultyStep = 0.5f;
    public float waveDuration = 20f;

    private float currentSpawnTime;
    private float spawnTimer;
    private float waveTimer;

    private void Start()
    {
        
        currentSpawnTime = baseSpawnTime;
    }

    private void Update()
    {
        if (spawners.Length == 0) return;

        spawnTimer += Time.deltaTime;
        waveTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnTime)
        {
            Spawn();
            spawnTimer = 0f;
        }

        if (waveTimer >= waveDuration)
        {
            IncreaseDifficulty();
            waveTimer = 0f;
        }
    }

    private void Spawn()
    {
        switch (spawnType)
        {
            case SpawnType.Enemy:
                SpawnEnemy();
                break;

            case SpawnType.Collectible:
                SpawnCollectible();
                break;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        Spawner spawner = spawners[Random.Range(0, spawners.Length)];
        GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Instantiate(enemy, spawner.transform.position, Quaternion.identity);
    }

    private void SpawnCollectible()
    {
        if (collectiblePrefabs.Length == 0) return;

        Spawner spawner = spawners[Random.Range(0, spawners.Length)];
        GameObject collectible = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

        Instantiate(collectible, spawner.transform.position, Quaternion.identity);
    }

    private void IncreaseDifficulty()
    {
        currentSpawnTime -= difficultyStep;
        currentSpawnTime = Mathf.Max(currentSpawnTime, minSpawnTime);

        Debug.Log("Wave terminada → Nuevo spawn time: " + currentSpawnTime);
    }
}