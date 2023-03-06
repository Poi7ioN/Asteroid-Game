using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This script is responsible for handling spawning system, Asteroids/Enemies.
public class ObstacleSpawner : MonoBehaviour
{
    [Header("Asteroid Spawn Settings")]
    public int _maxObstaclesToSpawn;                                         // max asteroids at a time in scene to spawn (increase this with time for difficulty)
    int _asteroidDefaultCount;                                              // default value for maxAsteroidCount when game restart
    [SerializeField] private List<AsteroidConfig>  _asteroidConfigFile;    // list of asteroid config files.
    [SerializeField] private float nextObstacleSpawnDelay = 1f;           // delay between asteroids spawns
    private int _currentNumOfObstaclesToSpawn;                           // current no of asteroids to spawn
    private int _spawnedAsteroidsCount;                                 // number of asteroids spawned 
    private Coroutine AsteroidSpawnerCoroutine;                        // coroutine to start asteroid spawner

    [Header("Enemy Spawn Settings")]
    public int _maxEnemiesToSpawn;                                        // max enemy at a time in scene to spawn ( increase this with time for diffculty)
    int _enemyDefaultCount;                                              // default value for maxEnemyCount when game restart
    [SerializeField] private List<EnemyConfigFile> _enemyConfigFile;    // list of enemy config files.
    [Range(5f,600f)] [SerializeField] private float _EnemySpawnDelay;  // delay between next wave of enemies
    [SerializeField] private float _delayBetweenEnemySpawn = 2;       // delay between loop of enemies 
    private int _spawnedEnemiesCount;                                // number of enemy spawned
    private Coroutine EnemySpawnerCoroutine;                        // coroutine to start enemy spawner.
    

    private void Awake()
    {
        Asteroid.OnAsteroidDestroyed += CheckForNewAsteroid;         //Subscribe to check asteroid destroyed event
        Asteroid.OnAsteroidSplit     += SplitAsteroidChilds;        //Subscribe to check asteroid split event
        Enemy.OnEnemyDestroyed       += CheckForNewEnemyWave;      //Subscribe to check enemy destroyed event
        GameManager.OnCheckGameState += CheckGameStart;           //Subscribe to check game state
        _asteroidDefaultCount         =_maxObstaclesToSpawn;     //Set default values on game restart
        _enemyDefaultCount            = _maxEnemiesToSpawn;     //Set default values on game restart
    }

    //Game start reset the variable to default values
    private void OnGameStart(bool state)
    {
        _maxObstaclesToSpawn = _asteroidDefaultCount;
        _maxEnemiesToSpawn   = _enemyDefaultCount;
        _currentNumOfObstaclesToSpawn = _maxObstaclesToSpawn;
        _spawnedAsteroidsCount = 0;
        _spawnedEnemiesCount   = 0;

        if (AsteroidSpawnerCoroutine != null)
        {
            // Stop the coroutine if it's still running from a previous playthrough
            StopCoroutine(AsteroidSpawnerCoroutine); 
        }

        if (EnemySpawnerCoroutine != null)
        {
            // Stop the coroutine if it's still running from a previous playthrough
            StopCoroutine(EnemySpawnerCoroutine); 
        }

        if(state)  // start spawning on game start true
        {
            AsteroidSpawnerCoroutine = StartCoroutine("SpawnAsteroids");
            EnemySpawnerCoroutine = StartCoroutine("SpawnEnemies");
        }
    }

    #region Asteroid Spawner
    //coroutine to spawn new asteroids.
    private IEnumerator SpawnAsteroids()
    {
        // Spawn multiple asteroids on each iteration, up to the maximum
        for (int i = 0; i < _currentNumOfObstaclesToSpawn; i++)
        {
            yield return new WaitForSeconds(nextObstacleSpawnDelay);

            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        //get random asteroid config file from list
        int randomAsteroid = UnityEngine.Random.Range(0, _asteroidConfigFile.Count);
        
        // Generate a random spawn direction using a normalized vector and multiply it with the asteroid's spawn distance
        Vector3 spawnDirection = UnityEngine.Random.insideUnitCircle.normalized * _asteroidConfigFile[randomAsteroid].GetAsteriodSpawnDistance;
        
        // Calculate the spawn position by adding the generated spawn direction to the spawner's position
        Vector3 spawnPosition = transform.position + new Vector3(spawnDirection.x, spawnDirection.y, 0);
        
        // Get a random angle variation using the current asteroid configuration file
        float angleVariation = _asteroidConfigFile[randomAsteroid].GetRandomAsteroidTrajectory;
        
        // Calculate the rotation using the angle variation and a forward-facing vector
        Quaternion rotation = Quaternion.AngleAxis(angleVariation, Vector3.forward);

        // Get an asteroid object from the object pooler using the asteroid prefab specified in the asteroid configuration file
        GameObject asteroid = ObjectPooler.SharedInstance.GetPooledObject(_asteroidConfigFile[randomAsteroid].GetAsteroidPrefab);
        asteroid.transform.SetPositionAndRotation(spawnPosition, rotation); //set position and rotation for spawned object
        asteroid.SetActive(true);
        
        // Update the asteroid's properties using the asteroid configuration file and the negative spawn direction to direct the asteroid towards the screen bounds
        asteroid.GetComponent<Asteroid>().UpdateAsteroidProperties(_asteroidConfigFile[randomAsteroid], rotation * -spawnDirection);
        
        // Increment the spawned asteroids count
        _spawnedAsteroidsCount++;
    }

    private void SplitAsteroidChilds(AsteroidConfig obj, float newSize, int newpoints, Vector2 pos)
    {
        for (int i = 0; i < 2; i++) //spawn two child asteroids from the data received from the parent asteroid.
        {
            GameObject asteroid = ObjectPooler.SharedInstance.GetPooledObject(obj.GetAsteroidPrefab);
            asteroid.transform.position = pos;
            asteroid.SetActive(true);
            //call asteroid split method and pass newsize,newpoints,random dir vector 
            asteroid.GetComponent<Asteroid>().SplitAsteroid(obj, newSize, newpoints, UnityEngine.Random.insideUnitCircle.normalized);
        }
        _spawnedAsteroidsCount++;
    }

    //when asteroid is destroyed checks for new asteroid to spawn
    public void CheckForNewAsteroid(GameObject asteroid)
    {
        //decrement the spanwed astoid count by 1
        _spawnedAsteroidsCount--;

        //clamp the count so that it won't go below 0
        _spawnedAsteroidsCount = Mathf.Clamp(_spawnedAsteroidsCount, 0, 1000);

        //if count of current spawned is less than max asteroids then spawn new asteroid
        if (_spawnedAsteroidsCount < _maxObstaclesToSpawn)  
        {
            //get no. of new asteroids to spanw by subtracting current from max asteroids.
            _currentNumOfObstaclesToSpawn = _maxObstaclesToSpawn - _spawnedAsteroidsCount;
            StopCoroutine(AsteroidSpawnerCoroutine);  //stop the coroutine and start new one
            AsteroidSpawnerCoroutine = StartCoroutine("SpawnAsteroids");
        }
    }

    #endregion

    #region Enemy Spawner

    //coroutine to spawn new enemies
    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(_EnemySpawnDelay); // This is the rate at which enemy ships will spawn
        
        // Spawn multiple enemies on each iteration, up to the maximum
        for (int i = 0; i < _maxEnemiesToSpawn; i++)
        {
            yield return new WaitForSeconds(_delayBetweenEnemySpawn);  // the is the delay between no. of ships (this value can be low default is 2)

            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int randomEnemy = UnityEngine.Random.Range(0, _enemyConfigFile.Count);
        Vector2 spawnDirection = UnityEngine.Random.insideUnitCircle.normalized * _enemyConfigFile[randomEnemy].GetEnemiesSpawnDistance;
        Vector3 spawnPosition = transform.position + new Vector3(spawnDirection.x, spawnDirection.y, 0);

        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject(_enemyConfigFile[randomEnemy].GetEnemyPrefab);
        enemy.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);   

        //Set enemy properties from the enemy config file
        enemy.GetComponent<Enemy>().SetEnemyDataOnSpawn(_enemyConfigFile[randomEnemy]);

        //Set enemy move speed on movement script
        enemy.GetComponent<Movement>().SetEnemyMoveDataOnSpawn(_enemyConfigFile[randomEnemy].GetEnemySpeed);
        enemy.SetActive(true);
        
        //increment enemy count with 1
        _spawnedEnemiesCount++;
    }

    //Check for new enemy to spawn
    public void CheckForNewEnemyWave(GameObject enemy)
    {
        //decrement enemy count when a enemy is killed
        _spawnedEnemiesCount--;
        
        //clamp the value to avoid negative number
        _spawnedEnemiesCount = Mathf.Clamp(_spawnedEnemiesCount, 0, 1000);

        if (_spawnedEnemiesCount == 0)  //if all enemies killed spawn new wave
        {
            StopCoroutine(EnemySpawnerCoroutine);
            EnemySpawnerCoroutine = StartCoroutine("SpawnEnemies");
        }
    }

    #endregion

    //Unsubscribe to events on gameobject disable 
    private void OnDisable()
    {
        Asteroid.OnAsteroidDestroyed -= CheckForNewAsteroid;
        Asteroid.OnAsteroidSplit     -= SplitAsteroidChilds;
        Enemy.OnEnemyDestroyed       -= CheckForNewEnemyWave;
        GameManager.OnCheckGameState -= CheckGameStart;
    }

    //Game start event from gamemanager to reset variable to default values
    public void CheckGameStart(bool state)
    {
        OnGameStart(state);
    }
}
