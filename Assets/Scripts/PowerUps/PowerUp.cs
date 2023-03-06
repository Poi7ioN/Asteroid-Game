using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Movement))]                //Add required movement script for child powerups
[RequireComponent(typeof(PowerUpProperties))]       //Add required Powerup Property script for child powerups

//This script is responsible for defining abstract methods from which child (powerups) classes can inherit
public abstract class PowerUp : MonoBehaviour
{
    private float _timer = 0f;                  
    private float _counter = 0f;
    protected bool _isPermanent;          //check if power up is temporary or permanent
    protected float _aliveTime;          //how long the power up hover during gameplay
    protected float _actingTime;        //how long the powerup stays alive when pikedup
    protected SpriteRenderer _sprite;
    protected Movement       _move;
    protected BoxCollider2D  _collider2D;
    protected bool _isPicked = false;     //check if powerup picked up

    public float Timer { get { return _timer; } set { _timer = value; } }
    public float Counter { get { return _counter; } set { _counter = value; } }

    public static event Action<bool> OnPowerDisable;      // event for sending powerup status

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //check if collided object is player or not
        PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();

        if (player != null)
        {
            ApplyPowerUp(player);       // Apply powerup on their respective child class
        }
    }

    protected virtual void Update() 
    {
        Counter += Time.deltaTime; // increment counter

        if (Counter >= Timer)
        {
            Counter = 0f;
            PowerUpHasExpired();  //call powerup expire method
        }
    }

    public virtual void SetDataOnEnable(PowerUpProperties data) //Set common Powerup properties
    {
        _aliveTime   = data.configData.GetPowerAliveTime;
        _actingTime  = data.configData.GetPowerActingTime;
        _isPermanent = data.configData.CheckPowerPerma;
        Timer        = _aliveTime;
    }

    public void NotifyPowerStatus(bool status)  // Notify Power spawner about the powerup status
    {
        OnPowerDisable?.Invoke(status);
    }

    // This method will be implemented in the derived powerup class with their respective logic for apply effect
    protected abstract void ApplyPowerUp(PlayerStats player);

    // This method will be implemented in the derived powerup class with their respective logic for expire effect
    public abstract void PowerUpHasExpired(); 

}