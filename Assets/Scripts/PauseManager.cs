using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour {

    public bool isPaused;
    public GameObject PauseCan;
    public GameObject ControlCan;

    // Use this for initialization
    void Start ()
    {
        Time.timeScale = 0;
        PauseCan = GameObject.Find("PauseCanvas");
        ControlCan = GameObject.Find("ControlCanvas");
        isPaused = true;
        PauseCan.SetActive(false);
        ControlCan.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        //Pause Function
        if (Input.GetKeyDown("tab"))
        {
            Pause();
            ControlCan.SetActive(false);
        }
    }

    public void Pause()
    {
        if (isPaused == true)
        {
            Time.timeScale = 1;
            isPaused = false;
            PauseCan.SetActive(false);
        }
        else
        {
            
            Time.timeScale = 0;
            isPaused = true;
            PauseCan.SetActive(true);
        }
    }
    public void control()
    {
        ControlCan.gameObject.SetActive(false);
    }
}
