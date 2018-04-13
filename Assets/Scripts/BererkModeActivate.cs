using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BererkModeActivate : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Globals.getBeserk() == false)
        {
            text.enabled = false;
        }

        if (Globals.getBeserk() == true)
        {
            text.enabled = true;
        }

    }
}
