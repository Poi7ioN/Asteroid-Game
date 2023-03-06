using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shield Power script
public class PowerUp_Shield : PowerUp
{
    PowerUpProperties data;                           //Shield power config data

    [Range(1, 100)] [SerializeField] private int _shieldHits = 1;   //set hit points of shield powerup

    PlayerStats player;

    private void Awake()  // get necessary component references
    {
         data       = GetComponent<PowerUpProperties>();
         player     = FindObjectOfType<PlayerStats>();
        _sprite     = GetComponent<SpriteRenderer>();
        _move       = GetComponent<Movement>();
        _collider2D = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateComponents(true);         //Update components on enable

        try //try catch is used because of object pooling conflict
        {
            SetDataOnEnable(data); // try to set data on abstract parent class on enable
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
        player.ActivateShieldPower(_shieldHits,true,this);  //Apply shield power up on player 

        AudioManager.Instance.PlayAudioClip(4, "Shield");  // play shield pick up audio

        if(_isPermanent) 
        {
            gameObject.SetActive(false);
        }
        else         //shield is a temporary powerup so don't disable after pickup
        {
            Counter = 0f;                  // reset counter 
            Timer   = _actingTime;        // set timer to acting time when pickedup 
            UpdateComponents(false);     // disable necessary components so that this instance stays alive in scene.
        }
    }

    //On Power up expired call
    public override void PowerUpHasExpired()
    {
        if (_isPicked)
        {
            // If player picked up the power, set deactivate shield powerup after acting time

            player.ActivateShieldPower(0, false, this);  
        }

        NotifyPowerStatus(false);          //notify power status on expiry
        gameObject.SetActive(false);       //disable the power up
    }
}
