using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {
    private static int character = 0;
	private static int mode = 0;
	private static int score = 0;
    private static bool beserkMode = false;

    public static void setCharacter(int characterCode)
    {
        Debug.Log("Setting character code");
        if(characterCode >= 0 && characterCode <= 2)
        {
            character = characterCode;
            Debug.Log("character set to " + characterCode);
        }
    }

	public static void setGameMode(int gameMode){
		Debug.Log ("Setting game mode.");

		if (gameMode == 0 || gameMode == 1) {
			mode = gameMode;
		}
	}
    public static int getCharacter()
    {
        Debug.Log("Returning character code " + character);
        return character;
    }

	public static int getGameMode()
	{
		Debug.Log ("Returning game mode " + mode);
		return mode;
	}

	public static void setScore(int newScore){
		score = newScore;
	}

	public static int getScore(){
		return score;
	}

    public static bool getBeserk()
    {
        return beserkMode;
    }

    public static void setBeserk(bool state)
    {
        beserkMode = state;
    }
}
