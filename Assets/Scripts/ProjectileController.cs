using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    public float projectileSpeed;
    private float angle;

    void Awake ()
    {
        
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * Time.deltaTime * projectileSpeed;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
        Debug.Log("Trigger Enter");
    }

    public void setAngle(float angle)
    {
        this.angle = angle;
    }
}
