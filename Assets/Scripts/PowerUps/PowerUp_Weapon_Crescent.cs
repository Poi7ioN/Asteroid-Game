using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Crescent weapon power up
public class PowerUp_Weapon_Crescent : PowerUp
{
    [SerializeField] private WeaponConfig weaponConfigData;  // Crescent power up config data

    PowerUpProperties data;
    Weapon playerWeapon;
    
    private void Awake()  // get necessary gameobject references in awake
    {
        data         = GetComponent<PowerUpProperties>();
        playerWeapon = FindObjectOfType<Weapon>();
        _sprite      = GetComponent<SpriteRenderer>();
        _move        = GetComponent<Movement>();
        _collider2D  = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateComponents(true);             //Update components on enable

        try   //try catch is used because of object pooling conflict
        {
            SetDataOnEnable(data);   // try to set data on abstract parent class on enable
        }
        catch
        {
            //Debug.Log("Object Pooled");
        }
    }

    private void UpdateComponents(bool value)
    {
        _isPicked           = !value;
        _move.enabled       = value;
        _sprite.enabled     = value;
        _collider2D.enabled = value;
    }

    //Apply effect to player
    protected override void ApplyPowerUp(PlayerStats player)
    {
        playerWeapon.SetWeaponType(weaponConfigData,PowerUpTypes.Moon);  //Apply moon weapon power up on player 

        if (_isPermanent)
        {
            gameObject.SetActive(false);
        }
        else   //Weapon type is a temporary powerup so don't disable after pickup
        {
            Counter = 0f;               // reset counter 
            Timer = _actingTime;       // set timer to acting time when pickedup 
            UpdateComponents(false);  // disable necessary components so that this instance stays alive in scene.
        }
    }

    //On Power up expired call
    public override void PowerUpHasExpired()
    {
        if (_isPicked)
        {
            // If player picked up the power, set deactivate weapon type powerup after acting time

            playerWeapon.SetDefaultWeapon();
        }
        NotifyPowerStatus(false);        //notify power status on expiry
        gameObject.SetActive(false);     //disable the power up
    }
}
