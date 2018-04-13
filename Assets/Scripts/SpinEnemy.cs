using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinEnemy : AI {
	public GameObject projectile;

	new private enum states{
		start,
		spin,
		rest
	}
	bool spin = true;
	private states state;
	private bool stateChange = true;
	public Animator anim;
	// Use this for initialization
	new void Start () {
		state = states.start;
		moveSpeed = 0;
		stopSpeed = 0;
		health = 10;
		damage = 10;
		base.Start ();
	}
	
	// Update is called once per frame
	new void Update () {
		if (stateChange) {
			switch (state) {
			case states.start:
				StartCoroutine (wait ());
				break;
			case states.spin:
				StartCoroutine (action ());
				attack ();
				break;
			case states.rest:
				StartCoroutine (rest ());
				break;
			}
		}
		base.Update ();
	}

	protected override void move(){
	}

	protected override void attack(){
		StartCoroutine (shoot ());
	}

	private IEnumerator wait(){
		stateChange = false;
		yield return new WaitForSeconds (1);
		state = states.spin;
		stateChange = true;

	}
	private IEnumerator action(){
		stateChange = false;
		int i = 0;

		StartCoroutine (time ());

		while(spin) {
			++i;
			gameObject.transform.Rotate (new Vector3 (0, 0, -i*Time.deltaTime));
			yield return new WaitForEndOfFrame ();
		}

		StartCoroutine (time ());
		while (spin) {
			gameObject.transform.Rotate (new Vector3 (0, 0, -i*Time.deltaTime));
			--i;
			yield return new WaitForEndOfFrame ();
		}

		state = states.rest;
		stateChange = true;
	}

	private IEnumerator time(){
		spin = true;
		yield return new WaitForSeconds (Random.Range(2.0f, 5.0f));
		spin = false;
	}

	private IEnumerator rest(){
		stateChange = false;
		anim.Play ("Rest");
		yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length);
		anim.Play ("Base");
		yield return new WaitForEndOfFrame ();
		state = states.start;
		stateChange = true;
	}

	private IEnumerator shoot(){
		yield return new WaitForSeconds (Random.Range(0.75f, 1.5f));

		while (state == states.spin) {
			GameObject proj = Instantiate (projectile, this.gameObject.transform.position, Quaternion.identity) as GameObject;
			proj.GetComponent<Rigidbody2D>().AddForce (transform.right * 750);
			yield return new WaitForSeconds (Random.Range(0.5f, 0.75f));
		}
	}

	public override void doDamage (int damage)
	{
		base.doDamage (damage);
	}
}
