using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour {

    public static int damage;
    ProjectileController pc;
	private static Player player = null;
	void Awake ()
    {
        pc = GetComponentInParent<ProjectileController>();
		if (player == null) {
			player = FindObjectOfType<Player> ();
		}
		damage = player.damage;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Projectile collided");
		if (other.gameObject.layer == LayerMask.NameToLayer ("Enemy")) {
			Debug.Log ("Enemy hit");
            // Golem has a separate AI script. Must check or else get Null Reference
            if(other.gameObject.transform.name.Equals("Golem(Clone)"))
            {
                other.gameObject.GetComponent<GolemAI>().doDamage(damage);
            }
            else
            {
                other.gameObject.GetComponent<AI>().doDamage(damage);
            }
		}
        Destroy(gameObject);
    }
}
