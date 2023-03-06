using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

//Game manager holds objects references and is responsible for handling game state and difficulty settings
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private bool _gameStarted;            //check game started
    private int _currentScore;           //current player score
    private int _newAsteroidThreshod;
    private int _newEnemyThreshod;
    private bool _checkForGameStartInput;   //input from user to start game

    [Header("Difficulty Settings")]
    [Tooltip("Set the score threshold at which the max number of asteroids will increase.")]
    [SerializeField] private int _obstacleIncreaseThreshold;
    [Range(1,20)][Tooltip("Set the count of asteroids to increase on every threshold")]
    [SerializeField] private int _asteroidIncreaseCount;   // default is 1, better to keep it at one or it will get too difficult

    [Tooltip("Set the score threshold at which the max number of enemies will increase.")]
    [SerializeField] private int _enemiesIncreaseThreshold;
    [Range(1, 20)][Tooltip("Set the count of enemies to increase on every threshold")]
    [SerializeField] private int _enemiesIncreaseCount;   // default is 1, better to keep it at one or it will get too difficult

    [Header("GameObject and Script References")]
    [SerializeField] private ObstacleSpawner obstacleSpawner; 
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _particleEffect;
    [SerializeField] private GameObject _tutorialObject;
    [SerializeField] private PlayableDirector _gameStartCutscene;

    public static event Action<bool> OnCheckGameState;   //Event that is responsible for sending game state to respective subscribed objects.

    private void Awake()
    {
        //singleton implementation
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        OnGameRestart();
    }

    public bool isGameStarted
    {
        get { return _gameStarted;  }
        set { _gameStarted = value; }
    }

    //General method for spawning particle effects and playing audio clips.
    public void DisableAndSpawnParticleEffect(Vector3 pos,float time,int index,string clipname)
    {
        GameObject effect = ObjectPooler.SharedInstance.GetPooledObject(_particleEffect);  // "destroy particle effect" reference from object pooler
        AudioManager.Instance.PlayAudioClip(index, clipname);
        effect.transform.position = pos;
        effect.SetActive(true);
        effect.GetComponent<DisableScript>().CallDisable(time);  //disable the effect with time delay
    }

    public void UpscaleDifficultyBasedOnScore(int points)
    {
        _currentScore = points;

        if (_currentScore - _newAsteroidThreshod >= 0)                              // check if current score is greater than or equal to _newAsteroidThreshod
        {
            obstacleSpawner._maxObstaclesToSpawn += _asteroidIncreaseCount;       // increase max obstacles
            _newAsteroidThreshod += _obstacleIncreaseThreshold;                  // add increase threshold to new threshold for next increment check
        }
        if (_currentScore - _newEnemyThreshod >= 0)                            // check if current score is greater than or equal to _newEnemyThreshod
        {
            obstacleSpawner._maxEnemiesToSpawn += _enemiesIncreaseCount; ;   // increase max obstacles
            _newEnemyThreshod += _enemiesIncreaseThreshold;                 // add increase threshold to new threshold for next increment check
        }
    }


    // This is a signal received from playable director when the starting cutscene is ended
    public void StartGameOnInputSignal() 
    {
        _checkForGameStartInput = true;
    }

    private void Update()
    {
        //when player inputs after cutscene the game starts.
        if(Input.GetKeyDown(KeyCode.Space) && _checkForGameStartInput)
        {
            _checkForGameStartInput = false;
            _playerInput.Shoot();                        // fire first user input bullets.
            SendGameStateEvent(true);                  
        }
    }

    //Reset Game Data
    public void OnGameRestart()
    {
        isGameStarted = false;
        _currentScore = 0;
        _checkForGameStartInput = false;
        _tutorialObject.SetActive(true);
        _newAsteroidThreshod = _obstacleIncreaseThreshold;
        _newEnemyThreshod = _enemiesIncreaseThreshold;
        SendGameStateEvent(false);
        _gameStartCutscene.Play();
    }

    //Invoke Game state events on subscribed objects.
    private void SendGameStateEvent(bool state)
    {
        OnCheckGameState?.Invoke(state);
    }
}

//Enumerated values for power up types.
public enum PowerUpTypes
{
    Health,
    Fuel,
    Life,
    Shield,
    Moon,
    Spread
}