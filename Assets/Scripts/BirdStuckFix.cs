using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdStuckFix : MonoBehaviour {

    private float maxStuckTime = 5;
    private float currentTimeInPos = 0;
    private Vector3 currentPos;
    private Vector3 prevPos = Vector3.zero;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        currentPos = transform.position;
        if(currentPos == prevPos)
        {
            currentTimeInPos += Time.deltaTime;
        } else
        {
            currentTimeInPos = 0;
        }
        if(currentTimeInPos >= maxStuckTime)
        {
            transform.position += new Vector3(0, 0.2f, 0);
        }
        prevPos = currentPos;
	}
}
