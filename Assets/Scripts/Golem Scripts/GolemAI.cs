using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAI : MonoBehaviour {
    private LevelGenerator levelGenerator;
    private GameObject player;
    //private Rigidbody2D rb;
    private CircleCollider2D cc;
    private GameObject projectile;
    private Animator anim;
    private SpriteRenderer render;

    protected static int gameMode = -1;

    private bool teleporting = false;
    private bool attacking = false;
    private int maxFireBeforeTeleport = 3;
    private int numberOfTimesFired = 0;

    protected bool facingRight = true;
    private float countUpTimer = 0;
    private float nextFire;
    private float nextTeleport;
    public float fireRate = 5f;
    public float teleportRate = 12f;
    public float jitterRate = 3f;
    private float time = 0;
    public float projectileSpeed = 15f;
    public int damage = 15;
	public float health = 10;
    private bool destroySanityCheck = false;

    private int spawnX, spawnY;
    private int maxRow, maxCol;

    // Use this for initialization
    void Start () {
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
        maxRow = levelGenerator.tileMap.GetLength(0);
        maxCol = levelGenerator.tileMap.GetLength(1);

       // rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        projectile = (GameObject)Resources.Load("Prefabs/blackProjectile");
        player = GameObject.FindGameObjectWithTag("Player");

        if (gameMode == -1)
        {
            gameMode = Globals.getGameMode();
        }
        switch (gameMode)
        {
            case 1:
                health = 2 * health;
                damage = 2 * damage;
                break;
        }

        teleporting = true;
        nextFire = Time.time + fireRate;
    }
	
    void FixedUpdate()
    {
        if (Time.time > nextFire && !attacking && numberOfTimesFired <= maxFireBeforeTeleport)
        {
            anim.SetTrigger("Attack");
            attacking = true;
            numberOfTimesFired++;
        }
        if (Time.time > nextTeleport && !teleporting)
        {
            anim.SetTrigger("TeleportOut");
            teleporting = true;
        }
    }

	// Update is called once per frame
	void Update () {

        //Look towards the player
        if (player.transform.position.x > transform.position.x)
        {
            render.flipX = true;
            facingRight = false;
        } else
        {
            render.flipX = false;
            facingRight = true;
        }

        
    }

    // adds a little variability to when an event triggers
    float jitter(float range)
    {
        return UnityEngine.Random.Range(-range, range);
    }

    public void doDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            anim.SetTrigger("Die");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The below states are called by the statemachingbehavior scripts attached to the animator of the object //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void beginTeleportingOut()
    {
        //Debug.Log("Beginning teleporting out");
        //Make unhittable
        cc.enabled = false;

        //Find location to teleport to

        do
        {
            spawnX = UnityEngine.Random.Range(0, maxCol);
            spawnY = UnityEngine.Random.Range(0, maxRow-1);
        } while (!(levelGenerator.tileMap[spawnY, spawnX] == 0 && levelGenerator.tileMap[spawnY+1, spawnX] == 1));

    }

    public void endTeleportingOut()
    {
        //Debug.Log("Ending teleporting out");
        teleporting = true;
        //Make sprite invisible
        render.enabled = false;
        //Move sprite to proper location. 0.3f added as an offset for better positioning
        transform.position = getSpawnLocation(spawnY, spawnX, maxRow);
        //Begin Teleporting in
        anim.SetTrigger("TeleportIn");
    }

    public void beginTeleportingIn()
    {
        //Debug.Log("Beginning teleporting in");
        teleporting = true;
        render.enabled = true;
    }

    public void endTeleportingIn()
    {
        //Debug.Log("Ending teleporting in");
        //Enemy is able to be hit again
        cc.enabled = true;
        teleporting = false;
        nextTeleport = Time.time + teleportRate + jitter(jitterRate);

        // Resets the number of times the golem can shoot per teleport
        numberOfTimesFired = 0;
        maxFireBeforeTeleport = UnityEngine.Random.Range(1, 4);
    }

    public void beginAttack()
    {

    }

    public void endAttack()
    {

        GameObject proj = Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
        EnemyProjectileController missile = proj.GetComponent<EnemyProjectileController>();

        Vector3 dir = player.transform.position - transform.position;
        dir = player.transform.InverseTransformDirection(dir);
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        //Debug.Log("angle = " + angle);

        missile.transform.Rotate(new Vector3(0,0,angle));
        missile.setAngle(angle);

        missile.setSpeedAndDamage(projectileSpeed, damage);

        missile.beginAction();

        nextFire = Time.time + fireRate + jitter(jitterRate);

        attacking = false;
    }

    public void destroyObject()
    {
        //Safety net in case function called multiple times
        if(!destroySanityCheck)
        {
            destroySanityCheck = true;
            Debug.Log("Golem Destroyed");
            LevelGenerator.score++;
            LevelGenerator.enemyCount--;
            Destroy(gameObject);
        }
    }

    public static Vector3 getSpawnLocation(int row, int col, int maxRow)
    {
        return new Vector3(col * 0.7f, ((maxRow - (int)row) * 0.7f) + 0.3f, 0);
    }

}
