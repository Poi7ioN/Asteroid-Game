using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Asteroid Config")]
public class AsteroidConfig : ScriptableObject
{
    [Header("Asteroid Spawn Info")]

    [Tooltip("Asteroid Prefab")]
    [SerializeField] GameObject _asteroidPrefab;

    [Tooltip("Asteroid Sprite Config")]
    [SerializeField]  SpriteConfigFile _asteroidSprites;

    [Tooltip("Score Points after Asteroid is Destroyed (evenly distributed among childs)")]
    [SerializeField] int _asteroidPoints;

    [Range(0.5f,7f)] [Tooltip("Maximum Asteroid Size")]
    [SerializeField]  float _maxAsteriodSize;

    [Range(1, 10)] [Tooltip("Maximum Hits to Destroy Asteroid")]
    [SerializeField]  int   _maxAsteriodHits;

    [Range(0.5f, 5f)] [Tooltip("Min Size of Asteroid till it can split into two asteroids")]
    [SerializeField]  float _minSizeToSplit;

    [Range(5f,50f)] [Tooltip("Move Speed of Parent Asteroid")]
    [SerializeField] float  _moveSpeed;

    [Range(1, 10)]
    [Tooltip("Damage to player based on size of asteroid")]
    [SerializeField] int _damageToPlayer;

    [Range(5f, 20f)]
    [Tooltip("How Far Asteroids to spawn (default is 20)")]
    [SerializeField] float _spawnDistance = 20f;

    [Range(10f, 70f)]
    [Tooltip("Angle at which asteroids are travelling (default is 50)")]
    [SerializeField] float _trajectoryVariance = 50;

    [Tooltip("Set True to get random values for asteroid. (size,speed)")]
    [SerializeField] bool _setRandomValues;


    // Get and Set Properties for the private variable.

    public GameObject GetAsteroidPrefab { get { return _asteroidPrefab; } }

    public float GetMinSizeOfAsteroidToSplit { get { return _minSizeToSplit; } }

    public Sprite GetRandomAsteroidSprite { get { return _asteroidSprites.GetRandomSprites; } }

    public float GetParentAsteroidSize { get  { return _maxAsteriodSize; } }

    public int GetAsteroidPoints { get { return _asteroidPoints; }}

    public float GetRandomAsteroidTrajectory { get { return UnityEngine.Random.Range(-_trajectoryVariance, _trajectoryVariance); } }

    public float GetAsteriodSpawnDistance { get { return _spawnDistance; } }

    public int GetAsteroidHits(float size)            //Get Asteroid Hits based on it's size
    {
        return Mathf.RoundToInt(size * _maxAsteriodHits / _maxAsteriodSize);
    }

    public int GetDamageToPlayer(float size)           //Get Asteroid's Damage to player based on it's size
    {
        return Mathf.RoundToInt(size * _damageToPlayer);
    }

    public float GetMoveSpeedOfAsteroid {  get { return _moveSpeed; } }

    public void GetRandomValues(out float size, out float speed)
    {
        if (_setRandomValues)  // if random values are set to true , generate size and speed randomly
        {
            size  = UnityEngine.Random.Range(0.5f, _maxAsteriodSize);
            speed = UnityEngine.Random.Range(5f, _moveSpeed);
        }
        else   // else return the config values
        {
            size  = _maxAsteriodSize;
            speed = _moveSpeed;
        }
    }
}
