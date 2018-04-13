using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class Border : MonoBehaviour {
	private GameObject levelMap;
	private LevelGenerator lg;

	public void Start(){
		levelMap = GameObject.Find("LevelGenerator");
		lg = levelMap.GetComponent<LevelGenerator>();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if ((other.tag == "mine" && other is CircleCollider2D) || other.tag == "spinProjectile"){
			return;
		}
        
        if(other.gameObject.name.Equals("Feisty(Attack)"))
        {
            // Tornado is not used in teleportation detection
            if (other is BoxCollider2D)
            {
                return;
            }
        }

        if (gameObject.tag == "left") {
			other.transform.position = new Vector3 ((float)(lg.tileMap.GetLength (1) - 1.25) * lg.width + lg.mapTranslation.x, other.transform.position.y, 0);
		} else if (gameObject.tag == "right") {
			other.transform.position = new Vector3 ((float)(lg.mapTranslation.x + .25 * lg.width), other.transform.position.y, 0);
		} else if (gameObject.tag == "bottom") {
			other.transform.position = new Vector3 (other.transform.position.x, (float)(lg.tileMap.GetLength (0) - 1.25) * lg.height + lg.mapTranslation.y, 0);
		} else if (gameObject.tag == "top") {
			other.transform.position = new Vector3 (other.transform.position.x, (float)(lg.height + lg.mapTranslation.y), 0);
		}
	}

}
