using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	public int damage;
	public Animator anim;
    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;
    public AudioClip explosionSound;
    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        damage = FindObjectOfType<MineEnemy> ().damage;
		StartCoroutine (Explode ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator Explode(){
        float vol = Random.Range(volLowRange, volHighRange);
        source.PlayOneShot(explosionSound, vol);
        anim.Play ("Explode");
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length);
		Destroy (gameObject);
    }

	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player") {
			Health playerHealth = other.GetComponent<Health> ();
			playerHealth.takeDamage (damage);
		}
		if (other.gameObject.transform.name.Equals ("Golem(Clone)")) {
			other.gameObject.GetComponent<GolemAI>().doDamage(damage);
		}
		else if(other.tag == "Enemy" || other.tag == "mine"){
			other.GetComponent<AI> ().doDamage (damage);
		}
	}
}
