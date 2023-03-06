using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//configuration file for powerup for spawnProbability,Alivetime,hoverSpeed,Acting time,Active/Passive
[CreateAssetMenu(menuName = "Power Spawner Config")]
public class PowerSpawnerConfig : ScriptableObject
{
    [Header("Power Type Info")]

    [Tooltip("Don't Forget to add the powerPrefab in object pooler")]
    public GameObject _powerPrefab;

    [Range(0f,1f)] [Tooltip("how often this power wills spawn")]
    [SerializeField] float _powerProbability;

    [Range(5f, 600f)] [Tooltip("how long this power will stay in scene")]
    [SerializeField] float _powerAliveTime;

    [Range(1f, 10f)]
    [Tooltip("Movement speed of powerup")]
    [SerializeField] float _powerHoverSpeed;

    [Range(1f, 20f)]
    [Tooltip("how long this power will act on player if its a temporary power.")]
    [SerializeField] float _powerActingTime;

    [Tooltip("Check if the power is permanent effect or not.")]
    [SerializeField] bool _isPermanent;


    //Getter and setter properties for private variables
    public float GetPowerSpawnProbability { get { return _powerProbability; } }

    public float GetPowerAliveTime { get { return _powerAliveTime; } }

    public float GetPowerMoveSpeed { get { return _powerHoverSpeed; } }

    public float GetPowerActingTime { get { return _powerActingTime; } }

    public bool CheckPowerPerma { get { return _isPermanent; } }

}
