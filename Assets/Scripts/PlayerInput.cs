using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for handling player input and movement.
public class PlayerInput : MonoBehaviour
{
    private float _playerThrustSpeed;                 // player forward thrust speed
    private float _playerTurnSpeed;                  // player rotation speed
    private bool _thrust;                           // check for forward movement
    private float _turn;                           // check for rotation
    private bool _isgameStarted;                  // check if game started

    //required component reference
    Rigidbody2D _rigidbody;
    Weapon playerWeapon;
    PlayerStats player;

    // Get references to the necessary components on this game object
    private void Awake()
    {
        GameManager.OnCheckGameState += CheckGameStart;   // subscribed to game start event
        player       = GetComponent<PlayerStats>();
        _rigidbody   = GetComponent<Rigidbody2D>();       
        playerWeapon = GetComponent<Weapon>();
    }

    void Update()
    {
        if (player._isPlayerAlive && _isgameStarted)  //Get player movement if gamestarted and player alive
        {
            CheckPlayerInputs();
        }
    }

    private void FixedUpdate()  //handle physics updated in fixedupdate when player alive and game started
    {
        if (player._isPlayerAlive && _isgameStarted) { AddForceToPlayer(); }
    }

    private void CheckPlayerInputs()
    {
        _thrust = Input.GetKey(KeyCode.UpArrow);                      //if up arrow pressed set thrust true
        _turn = Input.GetKey(KeyCode.LeftArrow) ? 1.0f : Input.GetKey(KeyCode.RightArrow) ? -1.0f : 0.0f; // Set _turn based on which arrow key is pressed
        if (Input.GetKeyDown(KeyCode.Space)) { Shoot(); }            // on space key press shoot weapon
    }

    public void Shoot()
    {
        playerWeapon.Shoot();
    }

    private void AddForceToPlayer()
    {
        if (_thrust && player.GetCurrentFuel > 0f) //move player on forward key and current fuel level
        {
            player.DepleteFuel();                      //update fuel level based on forward movement.
            Vector2 direction = transform.up;
            float speed = _playerThrustSpeed * 2f * Time.deltaTime;  // get acceleration
            Vector2 force = direction * speed;
            _rigidbody.AddForce(force, ForceMode2D.Impulse);  // apply force to rigidbody
        }
        if (_turn != 0.0f)
        {
            _rigidbody.AddTorque(_turn * _playerTurnSpeed); // check turn direction add torque
        }
    }

    private void OnDisable()  //unsubscribe to game start event on gameobject disable
    {
        GameManager.OnCheckGameState -= CheckGameStart;
    }

    public void CheckGameStart(bool state)  //game start method and setting necessary variable.
    {
        _isgameStarted = state;
        _playerThrustSpeed = player.playerConfigData.GetMaxPlayerSpeed;
        _playerTurnSpeed   = player.playerConfigData.GetMaxPlayerRotationSpeed;
    }

}
