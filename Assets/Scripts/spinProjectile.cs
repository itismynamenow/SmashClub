using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinProjectile : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			Health playerHealth = other.GetComponent<Health> ();
			playerHealth.takeDamage(FindObjectOfType<SpinEnemy> ().damage);
		}
	}
}
