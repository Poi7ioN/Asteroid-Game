using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//This script is responsible for handling the UI for game state like pause/play/final score/restart/quit
public class GameState : MonoBehaviour
{
    [SerializeField] GameObject _GameStateCanvas;
    [SerializeField] GameObject _PauseScreen;
    [SerializeField] GameObject _GameOverScreen;
    [SerializeField] TextMeshProUGUI _FinalScore;

    // On game over set game's simulation to pause and display final score
    public void OnGameOver(int score) 
    {
         Time.timeScale = 0;
        _PauseScreen.SetActive(false);
        _GameStateCanvas.SetActive(true);
        _GameOverScreen.SetActive(true);
        _FinalScore.text = score.ToString();
        CheckForHighScore(score);
    }

    // Check for new Highscore
    void CheckForHighScore(int newScore)
    {
        if(PlayerPrefs.GetInt("HighScore",0) < newScore)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_GameStateCanvas.activeInHierarchy)
        {
            //pause game with escape key
            PauseButton();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _PauseScreen.activeInHierarchy)
        {
            //resume game with escape key if already paused.
            ResumeGame();
        }
    }

    public void PauseButton()  // GUI pause function.
    {
        Time.timeScale = 0;
        _GameStateCanvas.SetActive(true);
        _PauseScreen.SetActive(true);
    }

    public void ResumeGame() // GUI Resume function.
    {
        Time.timeScale = 1;
        _GameStateCanvas.SetActive(false);
        _PauseScreen.SetActive(false);
    }

    public void RestartGame()  // GUI Restart function.
    {
        Time.timeScale = 1;
        _GameStateCanvas.SetActive(false);
        _PauseScreen.SetActive(false);
        _GameOverScreen.SetActive(false);
        GameManager.Instance.OnGameRestart();
    }

    public void MainMenu()    // Load to menu scene
    {
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()  // quit game.
    {
        Application.Quit();
    }
}
