using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Config")]
public class PlayerConfigFile : ScriptableObject
{
    [Header("Player Ship Info")]

    [Tooltip("Maximum Player Health")]
    [SerializeField] int _maxPlayerHealth;

    [Tooltip("Maximum Player Lives")]
    [SerializeField] int _maxPlayerLives;

    [Tooltip("Maximum Player Fuel")]
    [SerializeField] float _maxPlayerFuel;

    [Range(0.1f, 5f)] [Tooltip("Rate at which fuel depletes")]
    [SerializeField] float _fuelDepletionRate;

    [Range(1f, 10f)] [Tooltip("Maximum Player Speed")]
    [SerializeField] float _maxPlayerSpeed;

    [Range(0.1f, 10f)] [Tooltip("Maximum Player Rotation speed")] 
    [SerializeField] float _playerRotationSpeed;


    //Getter and setter properties for private variables.
    public int GetMaxPlayerHealth { get { return _maxPlayerHealth; } }

    public int GetMaxPlayerLives { get { return _maxPlayerLives; } }

    public float GetMaxPlayerFuel { get { return _maxPlayerFuel; } }

    public float GetMaxPlayerSpeed { get { return _maxPlayerSpeed; } }

    public float GetMaxPlayerRotationSpeed { get { return _playerRotationSpeed; } }

    public float GetFuelDepletionRate { get { return _fuelDepletionRate; } }

}
