using UnityEngine;

public class SpawnManager : MonoBehaviour
{
   
    public GameObject[] enemyPrefabs;

   
    public Spawner[] spawners;

   
    public float baseSpawnTime = 3f;       
    public float minSpawnTime = 0.25f;     
    public float difficultyStep = 0.5f;    
    public float waveDuration = 20f;      

    private float currentSpawnTime;
    private float spawnTimer = 0f;
    private float waveTimer = 0f;

    private void Start()
    {
        spawners = Object.FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        currentSpawnTime = baseSpawnTime;
    }

    private void Update()
    {
        if (spawners.Length == 0 || enemyPrefabs.Length == 0) return;

        spawnTimer += Time.deltaTime;
        waveTimer += Time.deltaTime;

       
        if (spawnTimer >= currentSpawnTime)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

       
        if (waveTimer >= waveDuration)
        {
            IncreaseDifficulty();
            waveTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Spawner spawner = spawners[Random.Range(0, spawners.Length)];
        GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Instantiate(enemy, spawner.transform.position, Quaternion.identity);
    }

    private void IncreaseDifficulty()
    {
        currentSpawnTime -= difficultyStep;
        currentSpawnTime = Mathf.Max(currentSpawnTime, minSpawnTime);

        Debug.Log("Wave terminada → Nuevo spawn time: " + currentSpawnTime);
    }
}