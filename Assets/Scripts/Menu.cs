using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _highScore;  //highscore text object reference

    // Start is called before the first frame update
    void OnEnable()
    {
        //set highscore from the playerpref data.
        _highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();  
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);  //load the gameplay scene.
    }
    
}
