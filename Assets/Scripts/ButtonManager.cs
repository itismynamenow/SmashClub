using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour {

    public void GameBttn(string name)
    {
       SceneManager.LoadScene(name);
    }

    public void quitGameBttn()
    {
        Application.Quit();
    }

    public void setCharacter(int characterCode)
    {
        Globals.setCharacter(characterCode);
    }

	public void setGameMode(int gameMode)
	{
		Globals.setGameMode(gameMode);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
