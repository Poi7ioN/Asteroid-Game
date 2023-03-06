using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//This script is responsible for updating player ui elements like health, fuel, lives , score
public class PlayerUICanvas : MonoBehaviour
{
    [Tooltip("Current Score of player")]   [SerializeField] private TextMeshProUGUI Score;
    [Tooltip("Remaining Lives of player")] [SerializeField] private TextMeshProUGUI Lives;
    [Tooltip("Health Bar Holder")]         [SerializeField] private Image _playerHealthBar;
    [Tooltip("Current Health of player")]  [SerializeField] private TextMeshProUGUI _currentHealth;
    [Tooltip("Gas Bar Holder")]            [SerializeField] private Image _playerGasBar;
    [Tooltip("Current Gas of player")]     [SerializeField] private TextMeshProUGUI _currentGas;
    [Tooltip("Tutorial Text Object ")]     [SerializeField] private GameObject _tutorialText;
    [Tooltip("UI Elements Parent")]        [SerializeField] private GameObject _UIParent;

    [SerializeField]private int Points;
    GameState gameState;
    
    // Start is called before the first frame update
    void Start()
    {
        gameState = GetComponent<GameState>(); 
    }

    private void OnEnable()
    {
        Enemy.OnUpdateScoreEvent         += OnUpdatePointsEvent;      // subscribe to add points event when enemy killed
        Asteroid.OnUpdateScoreEvent      += OnUpdatePointsEvent;     // subsribe to add points event when asteroid destroyed
        PlayerStats.OnUpdateHealthBar    += UpdateHealthData;       // subsribe to update player health
        PlayerStats.OnUpdateGasBar       += UpdateGasData;         // subsribe to update player fuel
        PlayerStats.OnUpdatePlayerLife   += OnUpdatePlayerLives;  // subsribe to update player lives
        PlayerStats.OnGameOver           += OnGameOver;          // subsribe to game over call
        GameManager.OnCheckGameState     += OnGameStart;        // subscribe to game state event
    }

    private void OnGameOver() //pass points to game over screen
    {
        gameState.OnGameOver(Points);
    }

    private void OnUpdatePointsEvent(int newPoint)  // add new points on event listen.
    {
        Points += newPoint;
        GameManager.Instance.UpscaleDifficultyBasedOnScore(Points);  //increase difficulty based on score
        Score.text = (Points).ToString();
    }

    private void OnUpdatePlayerLives(int newLife)  // update lives on event listen.
    {
        Lives.text = newLife.ToString();
    }

    private void UpdateHealthData(int currentHealth, int TotalHealth)  // udpte player health
    {
        float fillAmount    = (float)currentHealth / TotalHealth;
        _currentHealth.text = currentHealth.ToString();
        _playerHealthBar.fillAmount = fillAmount;                   //set image fill amount 
    }

    private void UpdateGasData(float currentGas, float TotalGas)  //update player fuel
    {
        float fillAmount = currentGas / TotalGas;
        _currentGas.text = currentGas.ToString("0");
        _playerGasBar.fillAmount = fillAmount;           //set image fill amount
    }

    // reset data on new game
    private void OnGameStart(bool obj)
    {
        _tutorialText.SetActive(false);
        Points = 0;
        OnUpdatePointsEvent(0);
        _UIParent.SetActive(obj);
    }

    private void OnDisable()  //unsubscribe to events on gameobject disable.
    {
        Enemy.OnUpdateScoreEvent         -= OnUpdatePointsEvent;
        Asteroid.OnUpdateScoreEvent      -= OnUpdatePointsEvent;
        PlayerStats.OnUpdateHealthBar    -= UpdateHealthData;
        PlayerStats.OnUpdateGasBar       -= UpdateGasData;
        PlayerStats.OnUpdatePlayerLife   -= OnUpdatePlayerLives;
        PlayerStats.OnGameOver           -= OnGameOver;
        GameManager.OnCheckGameState     -= OnGameStart;
    }
}
