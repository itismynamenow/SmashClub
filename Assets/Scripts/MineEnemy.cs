using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineEnemy : AI {

	public Animator anim;
	public GameObject explosion;
    private bool sanityLock = false;

	// Use this for initialization
	void Start () {
		moveSpeed = 0;
		stopSpeed = 0;
		health = 10;
		damage = 35;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			attack ();
		}

	}

	public override void doDamage(int damage){
		health -= damage;
		if (health <= 0 && !sanityLock) {
            sanityLock = true;
            StopAllCoroutines();
			Instantiate (explosion, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, 0), Quaternion.identity);
            LevelGenerator.enemyCount--;
            Debug.Log("Enemy Count " + LevelGenerator.enemyCount);
            LevelGenerator.score++;
            Destroy (gameObject);
		} else if(!sanityLock){
            sanityLock = true;
			StartCoroutine (Die ());
		}
	}
	protected override void move(){
		return;
	}

	protected override void attack(){
        if (!sanityLock)
        {
            sanityLock = true;
            StartCoroutine(Die());
        }
		
	}

	private IEnumerator Die()
	{
		anim.Play ("MineExplode");
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length);
		Instantiate (explosion, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, 0), Quaternion.identity);
        LevelGenerator.enemyCount--;
        LevelGenerator.score++;
        Destroy (gameObject);
        StopAllCoroutines();
	}
}
