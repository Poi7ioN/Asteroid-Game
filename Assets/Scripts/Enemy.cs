using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour,IDamagable
{
    private int _maxEnemyLives;               //enemy hit points
    private int _enemyDamageToPlayer;         //damage to player
    private int _enemyPoints;                //points to player on enemy killed
    private float _enemyTimeBetweenShots;    //delay between next fire
    float shotCounter;                      //shot counter to check for next fire

    // Private variables to store references to components and game objects
    Rigidbody2D _rigidbody2D;
    SpriteRenderer _spriteRenderer;
    PlayerStats _player;
    Weapon _enemyWeapon;

    public static event Action<int> OnUpdateScoreEvent;        //Update Score Event which is observed in UI
    public static event Action<GameObject> OnEnemyDestroyed;   //Update Score Event which is observed in Obstacle spawner

    private void Awake()
    {
        // Get references to the necessary components on this game object
         _player        = FindObjectOfType<PlayerStats>();
         _enemyWeapon   = GetComponent<Weapon>();
        _rigidbody2D    = GetComponent<Rigidbody2D>();      
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Populate Enemy Data from the enemy config file.
    public void SetEnemyDataOnSpawn(EnemyConfigFile configData)
    {
        _spriteRenderer.sprite = configData.GetRandomEnemySprite;
        configData.GetRandomEnemyValues(out _maxEnemyLives,out _enemyDamageToPlayer,out _enemyTimeBetweenShots,out _enemyPoints);
    }

    private void Update()
    {
        ShotingCounter();   

        if (_player != null)
        {
            // Calculate the direction from the asteroid's position to the player's position
            Vector3 dir = _player.transform.position - transform.position;
            
            // Calculate the angle between the direction vector and the x-axis in degrees using atan2 and convert it to degrees
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            // Set the rotation of the asteroid's transform to face the player using a Quaternion
            // The adjustment angle of -90 degrees is added to make the asteroid face the player properly
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    private void ShotingCounter()
    {
        shotCounter -= Time.deltaTime;   // decrement the shot counter for next fire call check
        if (shotCounter <= 0)
        {
            shotCounter = _enemyTimeBetweenShots;
            _enemyWeapon.Shoot();   // shoot player
        }
    }

    //This is a interface implemented method which is called when the enemy takes damage.
    public void Damage(int dmg)
    {
        _maxEnemyLives -= dmg;          // Reduce the enemy's hit points by the damage amount
        if (_maxEnemyLives <= 0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.DisableAndSpawnParticleEffect(transform.position, 3f, 3, "Ship");
            
            //Invoke event to udpate score on ui
            OnUpdateScoreEvent?.Invoke(_enemyPoints);   
            
            //Invoke event to update obstacle spawner that enemy is destroyed.
            OnEnemyDestroyed?.Invoke(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PlayerStats component of the colliding object, if it has one
        PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
        
        // If the colliding object is the player
        if (player != null)
        {
            // Damage the player with the asteroid's damage
            player.Damage(_enemyDamageToPlayer);
        }
    }
}
