using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour {

    private bool start = false;
    private float projectileSpeed;
    private int damage;
    private float angle;

    // Use this for initialization
    void Start()
    {
    }

    void FixedUpdate()
    {
        if(start)
        {
            // Move towards whatever position the player was at time of fire
            transform.position += new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * Time.deltaTime * projectileSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Player is hit!
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player hit");
            other.gameObject.GetComponent<Health>().takeDamage(damage);
            //Destroys self
            Destroy(gameObject);
        }
    }
    public void setAngle(float angle)
    {
        this.angle = angle;
    }

    public void beginAction()
    {
        start = true;
    }

    public void setSpeedAndDamage(float speed, int damage)
    {
        projectileSpeed = speed;
        this.damage = damage;
    }

}