using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Config")]
public class EnemyConfigFile : ScriptableObject
{
    [Header("Enemy Spawn Info")]

    [Tooltip("Enemy Prefab")]
    [SerializeField] GameObject _enemyPrefab;

    [Tooltip("Enemy Sprite Config File")]
    [SerializeField] SpriteConfigFile _enemySpriteConfig;

    [Tooltip("Enemy Bullet Sprites")]
    [SerializeField] List<Sprite> _enemyBullets;

    [Range(1, 50)]
    [Tooltip("Points on killing Enemy")]
    [SerializeField] int _enemyPoints;

    [Range(1, 10)]
    [Tooltip("Enemy Hits to destory")]
    [SerializeField] int _maxEnemyHits;

    [Range(1, 10)]
    [Tooltip("Enemy Damage to Player")]
    [SerializeField] int _EnemyDamageToPlayer;

    [Range(2f, 7f)]
    [Tooltip("Maximum Enemy Fire rate")]
    [SerializeField] float _timeBetweenShots;

    [Range(1f, 10f)]
    [Tooltip("Enemy Move Speed")]
    [SerializeField] float _enemyMoveSpeed;

    [Range(5f, 20f)]
    [Tooltip("How Far Enemies to spawn (default is 10)")]
    [SerializeField] float _spawnDistance = 10f;

    [SerializeField] bool _getRandomValues;


    // Getter and Setter properties for private variable.
    public GameObject GetEnemyPrefab { get { return _enemyPrefab; } }

    public Sprite GetRandomEnemySprite { get { return _enemySpriteConfig.GetRandomSprites; } }

    public float GetEnemiesSpawnDistance { get { return _spawnDistance; } }

    public int GetEnemyPoints { get { return _enemyPoints; } }

    /////These following properties are kept random.

    public int GetEnemyHits { get { return _maxEnemyHits; } }

    public int GetEnemyDamage { get { return _EnemyDamageToPlayer; } }

    public float GetEnemyTimeBetweenShots { get { return _timeBetweenShots; } }

    public float GetEnemySpeed{ get { return _enemyMoveSpeed; } }
    


    public void GetRandomEnemyValues(out int maxEnemyHits, out int enemyDamageToPlayer, out float timeBetweenShots, out int points)
    {
        if (_getRandomValues)  // on random value is selected , generate hit points, damage to player, fire delay and points randomly.
        {
            maxEnemyHits        = Random.Range(1, _maxEnemyHits + 1);
            enemyDamageToPlayer = Random.Range(1, _EnemyDamageToPlayer);
            timeBetweenShots    = Random.Range(2f, _timeBetweenShots);
            points              = GetPointsBasedOnHits(maxEnemyHits);
        }
        else  //else return the config values
        {
            maxEnemyHits        = _maxEnemyHits;
            enemyDamageToPlayer = _EnemyDamageToPlayer;
            timeBetweenShots    = _timeBetweenShots;
            points              = _enemyPoints;
        }
    }

    private int GetPointsBasedOnHits(int Hits) //get points of enemies based on hit points.
    {
        return Hits * 10;
    }
}
