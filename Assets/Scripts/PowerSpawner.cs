using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This script is responsible for handling powerup spawn system
public class PowerSpawner : MonoBehaviour
{
    [SerializeField] private List<PowerSpawnerConfig> powerupPrefabs;             // list of power config files

    [Range(10f, 600f)] [SerializeField] private float _powerSpawnDelay;         // next power up spawn delay 
    [Range(0f, 1f)]    [SerializeField] private float _powerSpawnProbability;  //power up probabilty when target destroyed
    
    private bool _checkGameStart = false;    //check game start
    private float powerCounter;             //count down to spawn powerup
    private float totalProbability = 0;    //total probabilties of all powerups
    private bool isPowerAlive = false;    //Check if powerup is spawned during gameplay

    private void Awake()
    {
        Asteroid.OnSpawnPower        += CheckForPowerSpawn;   //subscribe event to check for spawning powerup on asteroid destroyed
        PowerUp.OnPowerDisable       += UpdatePowerCounter;  //subscribe event to update power alive bool 
        GameManager.OnCheckGameState += CheckGameStart;     //subscribe to game start event
    }

    private void Update()
    {
        if (_checkGameStart)
        {
            SpawnPowerCounter();  // start spawn power up counter after game started
        }
    }

    private void SpawnPowerCounter()
    {
        powerCounter += Time.deltaTime;     //increment spawn counter

        if (powerCounter >= _powerSpawnDelay) 
        {
            powerCounter = 0;            
            Vector2 spawnDirection = UnityEngine.Random.insideUnitCircle.normalized * 10f;  //get a random point for spawning
            Vector3 spawnPosition = transform.position + new Vector3(spawnDirection.x, spawnDirection.y, 0);
            SpawnPowerup(spawnPosition);
        }
    }

    //Check for Powerup spawn when asteroid is destroyed 
    private void CheckForPowerSpawn(Vector3 pos)
    {
        float randomProb = UnityEngine.Random.Range(0f, 1f);

        if (randomProb < _powerSpawnProbability)
        {
            powerCounter = 0;    //reset the power counter when new powerup is spawned
            SpawnPowerup(pos);
        }
    }

    private void SpawnPowerup(Vector3 position)
    {
        if (isPowerAlive) { return; }  // if power up is already alive then dont spawn new powerup.

        
        float cumulativeProbability = 0;
        float randomProb = UnityEngine.Random.Range(0f, totalProbability); // get a random value with range between 0 and totalprobabilities

        //loop through all powerups
        for (int i = 0; i < powerupPrefabs.Count; i++)
        {
            //add probability value of each powerup one by one
            cumulativeProbability += powerupPrefabs[i].GetPowerSpawnProbability;

            //if random value is less than cumulative spawn powerup with that correspoing probability
            if (randomProb < cumulativeProbability)  
            {
                SpawnRandomPower(position, i);
                break;
            }
        }
    }

    private void SpawnRandomPower(Vector3 position, int i)
    {
        // Get an powerup object from the object pooler using the powerup prefab specified in the powerup configuration file
        GameObject powerUp = ObjectPooler.SharedInstance.GetPooledObject(powerupPrefabs[i]._powerPrefab);
        
        //set position and rotation for powerup prefab
        powerUp.transform.SetPositionAndRotation(position, Quaternion.identity);

        //set powerup properties with respective config data
        powerUp.GetComponent<PowerUpProperties>().configData = powerupPrefabs[i];

        //set powerup movement speed from config data
        powerUp.GetComponent<Movement>().SetEnemyMoveDataOnSpawn(powerupPrefabs[i].GetPowerMoveSpeed);
        powerUp.SetActive(true);
        UpdatePowerCounter(true);
    }

    //unsubscribe to events on gameobject disable.
    private void OnDisable()  
    {
        Asteroid.OnSpawnPower        -= CheckForPowerSpawn;
        PowerUp.OnPowerDisable       -= UpdatePowerCounter;
        GameManager.OnCheckGameState -= CheckGameStart;
    }

    public void UpdatePowerCounter(bool value)  //update power alive status
    {
        isPowerAlive = value;
    }

    private void OnGameStart()  // reset data on new game
    {
        powerCounter = 0f;

        for (int i = 0; i < powerupPrefabs.Count; i++)
        {
            totalProbability += powerupPrefabs[i].GetPowerSpawnProbability;
        }
        UpdatePowerCounter(false);
    }

    public void CheckGameStart(bool state)  // game start event to reset data
    {
        _checkGameStart = state;
        if (state) { OnGameStart(); }
    }
}
