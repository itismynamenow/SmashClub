using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour {
	public Text score;
	public static int scoreCount;
	// Use this for initialization
	void Start () {
        Globals.setBeserk(false);
    }
	
	// Update is called once per frame
	void Update () {
        if(score.name.Equals("Score"))
        {
            score.text = "Score: " + scoreCount;
        }
        else if(score.name.Equals("Highscore"))
        {
            if (Globals.getGameMode() == 0)
            {
                score.text = "Highscore: " + PlayerPrefs.GetInt("High Score Normal");
            }
            else
            {
                score.text = "Highscore: " + PlayerPrefs.GetInt("High Score Hard");
            }
        }
        else if(score.name.Equals("menuNormalHighscore"))
        {
            score.text = "Normal Mode Highscore: " + PlayerPrefs.GetInt("High Score Normal");
        }
        else if (score.name.Equals("menuHardHighscore"))
        {
            score.text = "HARDMODE Highscore: " + PlayerPrefs.GetInt("High Score Hard");
        }

    }
}
