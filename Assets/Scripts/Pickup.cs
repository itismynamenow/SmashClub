using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour {
	public Text pickup;
	public static string powerup = "";
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		pickup.text = "Active Powerup: " + powerup;
	}
}
