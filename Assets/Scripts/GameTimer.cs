using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameTimer : MonoBehaviour {

    public Text Timer;
    public float maxTime;
    public float currentTime;
    bool TimeLoad = false;
	private int gameMode;
    // Use this for initialization
    void Start () {
		currentTime = maxTime;
		Timer.text = currentTime.ToString();
		TimeLoad = true;
    }
	
	// Update is called once per frame
	void Update () {
		if (currentTime > 0)
		{
			currentTime -= Time.deltaTime;
			Timer.text = currentTime.ToString();
		}
		else if (currentTime <= 0 && TimeLoad == true)
		{
            Debug.Log("Game Ended. Gamemode was " + Globals.getGameMode());
			Globals.setScore (ScoreCounter.scoreCount);
            if(PlayerPrefs.GetInt("High Score Normal") < ScoreCounter.scoreCount && Globals.getGameMode() == 0)
            {
                Debug.Log("New Highscore");
                PlayerPrefs.SetInt("High Score Normal", ScoreCounter.scoreCount);
            }
            else if(PlayerPrefs.GetInt("High Score Hard") < ScoreCounter.scoreCount && Globals.getGameMode() == 1)
            {
                Debug.Log("New Highscore");
                PlayerPrefs.SetInt("High Score Hard", ScoreCounter.scoreCount);
            }
            SceneManager.LoadScene("GameOver");
		}
    }
}
