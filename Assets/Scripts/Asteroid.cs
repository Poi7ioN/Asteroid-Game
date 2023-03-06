using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This script holds asteroid properties in the game
public class Asteroid : MonoBehaviour, IDamagable
{
    // Private variables to store references to components and game objects
    private int _asteroidHits;
    private float _asteroidSize;
    private float _asteroidSpeed;
    private float _minStepsToSplit;
    private float _maxSizeToSplit;
    private int _asteroidPoints;
    private int _damageToPlayer;

    Rigidbody2D _rigidbody;
    SpriteRenderer _sprite;
    AsteroidConfig asteroidConfigData;
    ObstacleSpawner obstacleSpawner;

    public static event Action<int> OnUpdateScoreEvent;                              //Update Score Event which is observed in UI
    public static event Action<GameObject> OnAsteroidDestroyed;                     //Asteroid Destroyed Event which is observed in obstacle spawner.
    public static event Action<AsteroidConfig,float,int,Vector2> OnAsteroidSplit;  //Asteroid Split Event which is observed in obstacle spawner.
    public static event Action<Vector3> OnSpawnPower;                             //Power Spawn Event which is observed in Power spawner.

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get references to the necessary components on this game object
        _rigidbody      = GetComponent<Rigidbody2D>();
        _sprite         = GetComponent<SpriteRenderer>();
        obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
    }

    // This method updates the properties of the asteroid using the specified config file and direction vector
    public void UpdateAsteroidProperties(AsteroidConfig configFile, Vector2 dir)
    {
        asteroidConfigData    = configFile;                                            // Store a reference to the provided asteroid config file
        _sprite.sprite        = asteroidConfigData.GetRandomAsteroidSprite;            // Get a random sprite from the config file and set it on this asteroid
        transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.value * 360f);   // Randomize the asteroid's rotation
        asteroidConfigData.GetRandomValues(out _asteroidSize, out _asteroidSpeed);    // Get a random asteroid size and speed from the config file and set it on this asteroid
        transform.localScale  = Vector3.one * _asteroidSize;
        _asteroidHits         = asteroidConfigData.GetAsteroidHits(_asteroidSize);    // Calculate the number of hits the asteroid can take based on its size
        _rigidbody.mass       = _asteroidSize;                                        // Set the asteroid's mass based on its size
        _damageToPlayer       = asteroidConfigData.GetDamageToPlayer(_asteroidSize);  // Calculate the amount of damage the asteroid does to the player based on its size
        _minStepsToSplit      = asteroidConfigData.GetMinSizeOfAsteroidToSplit;        // Get the minimum asteroid size till the asteroid can be split into
        _asteroidPoints       = asteroidConfigData.GetAsteroidPoints;                  // Get the number of points this asteroid is worth based on its size
        _rigidbody.AddForce(dir * _asteroidSpeed);                                      // Apply force to the asteroid in the specified direction with the specified speed
    }

    // This method splits the asteroid into two smaller asteroids from the data received from its parent asteroid
    public void SplitAsteroid(AsteroidConfig configFile,float size,int point, Vector2 dir)
    {
        asteroidConfigData    = configFile;
        _asteroidSize         = size;
        _sprite.sprite        = asteroidConfigData.GetRandomAsteroidSprite;
        transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.value * 360f);
        transform.localScale  = Vector3.one * size;
        _asteroidHits         = asteroidConfigData.GetAsteroidHits(size);
        _rigidbody.mass       = size;
        _damageToPlayer       = asteroidConfigData.GetDamageToPlayer(size);
        _minStepsToSplit      = asteroidConfigData.GetMinSizeOfAsteroidToSplit;
        _asteroidPoints       = point;
        ApplyExplosionForce(_rigidbody);
    }

    // This method applies an explosion force to the given Rigidbody2D object
    private void ApplyExplosionForce(Rigidbody2D rb)
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;  // Generate a random direction vector using insideUnitCircle and normalize it to get a unit vector
        float randomMagnitude = UnityEngine.Random.Range(3f, 6f);                  // Generate a random magnitude between 3 and 6
        Vector2 randomVelocity = randomDirection * randomMagnitude;                 // Multiply the random direction vector by the random magnitude to get a random velocity vector
        rb.velocity = randomVelocity;                                              // Set the velocity of the Rigidbody2D object to the random velocity vector
    }

    // This method is and interface implementation which check for collision and call damage method
    public void Damage(int dmg)
    {
        _asteroidHits -= dmg;            // Reduce the asteroid's hit points by the damage amount
        if (_asteroidHits <= 0)          
        {
            gameObject.SetActive(false);
            GameManager.Instance.DisableAndSpawnParticleEffect(transform.position,3f, 2, "Asteroid");
            if (_asteroidSize * 0.5f >= _minStepsToSplit)  // split asteroid if greater than min size specfied in config file
            {
                Vector2 position = transform.position;
                position += UnityEngine.Random.insideUnitCircle * 0.5f;
                // Invoke the OnAsteroidSplit event with the asteroid's configuration data, half the asteroid's size and points, and the new position
                OnAsteroidSplit?.Invoke(asteroidConfigData,_asteroidSize/2f,_asteroidPoints/2,position); 
            }
            else
            {
                //destory smallest part of asteroid and score points
                // Invoke the OnAsteroidDestroyed event with the asteroid game object
                OnAsteroidDestroyed?.Invoke(gameObject);
                // Spawn a power-up at the asteroid's position
                OnSpawnPower?.Invoke(transform.position);
                // Update the score with the asteroid's points
                OnUpdateScoreEvent?.Invoke(_asteroidPoints);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the PlayerStats component of the colliding object, if it has one
        PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
        // If the colliding object is the player
        if (player != null)
        {
            // Damage the player with the asteroid's damage
            player.Damage(_damageToPlayer);
        }
    }
}
