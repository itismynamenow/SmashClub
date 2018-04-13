using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FeistyKittyCharacter : Player
{
    // Shooting projectile variables
    private GameObject projectile;
    private RectTransform rect;
    private float nextFire;
    public float attackTime;
    private float countdownAttack;
    private Health health;
    private BoxCollider2D boxCollider;
    private AreaEffector2D areaEffector;
    private float attackDelay = 0.5f;
    private bool isBeserk = false;


    void Update()
    {
        if (countdownAttack + attackTime < Time.time)
        {
            gameObject.name = "Feisty(Clone)";
            anim.SetBool("attacking", false);
            boxCollider.enabled = false;
            areaEffector.enabled = false;
            boxCollider.isTrigger = false;
        }
        base.Update();

        if(!isBeserk && health.HealthRatio <= 0.21f)
        {
            isBeserk = true;
            Globals.setBeserk(true);
            goBeserk();
        }
        else if(isBeserk && health.HealthRatio > 0.21f)
        {
            isBeserk = false;
            Globals.setBeserk(false);
            undoBeserk();
        }
    }


    void Start()
    {
        gameObject.transform.tag = "Player";
        playerHealth = GetComponent<Health>();
        moveSpeed = 5;
        fireRate = 4.0f;
        jumpHeight = 15;
        wallJumpHeight = 7;
        maxJumpCount = 2;
        maxWallJumpCount = 4;
        waveDashSpeed = 8;
        waveDashDuration = 0.3f;
        waveDashDelay = 1.5f;
        attackTime = 1.5f;


        rect = GetComponent<RectTransform>();
        health = GetComponent<Health>();

        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        anim.enabled = true;

        levelMap = GameObject.Find("LevelGenerator");
        lg = levelMap.GetComponent<LevelGenerator>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = false;
        areaEffector = GetComponent<AreaEffector2D>();
        for (int x = 0; x < lg.tileMap.GetLength(1); x++)
        {
            for (int y = 0; y < lg.tileMap.GetLength(0); y++)
            {
                if (lg.tileMap[y, x] == 2)
                {
                    float xStartPosition = (x * lg.blockSize) + lg.mapTranslation.x;
                    float yStartPosition = (lg.tileMap.GetLength(0) - y) * lg.blockSize + lg.mapTranslation.y;
                    tr.position = new Vector3(xStartPosition, yStartPosition, 0);
                }
            }
        }

        base.Start();
    }

    protected override void attack()
    {
        // Checks if delay is active
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            anim.SetBool("attacking", true);
            countdownAttack = Time.time;
            health.setInvincible(true, attackTime);
            boxCollider.enabled = true;
            areaEffector.enabled = true;
            boxCollider.isTrigger = true;
            gameObject.name = "Feisty(Attack)";
            CooldownController.Cooldown3();
        }
    }

    protected override void usePower()
    {

    }

    //Used with attack
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Enemy hit");
            if (other.gameObject.transform.name.Equals("Golem(Clone)"))
            {
                other.gameObject.GetComponent<GolemAI>().doDamage(damage);
            }
            else
            {
                other.gameObject.GetComponent<AI>().doDamage(damage);
            }
        }

        if (other.tag == "chest")
        {
            string powerup = lg.getDrop();
            Destroy(other.gameObject);
            IEnumerator coroutine = base.setPowerup(powerup);
            StartCoroutine(coroutine);

        }
        else if (other.tag == "sand")
        {
            currentTerrain = terrain.sand;
        }
        else if (other.tag == "ground")
        {
            currentTerrain = terrain.ground;
        }
        else if (other.tag == "ice")
        {
            currentTerrain = terrain.ice;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        attackDelay -= Time.deltaTime;
        if (boxCollider.enabled && attackDelay <= 0)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.Log("Enemy hit");
                if (other.gameObject.transform.name.Equals("Golem(Clone)"))
                {
                    other.gameObject.GetComponent<GolemAI>().doDamage(damage);
                }
                else
                {
                    other.gameObject.GetComponent<AI>().doDamage(damage);
                }
            }
            attackDelay = 0.5f;
        }
    }

    public bool isAttacking()
    {
        return areaEffector.enabled;
    }

    public float getDashCool()
    {
        return waveDashDelay;
    }
    public float getAttackCool()
    {
        return fireRate;
    }

    private void goBeserk()
    {
        moveSpeed *= 2f;
        fireRate /= 1.5f;
        jumpHeight += 3;
        wallJumpHeight += 3;
        maxJumpCount += 3;
        maxWallJumpCount += 2;
        waveDashSpeed *= 2;
        waveDashDuration += 0.2f;
        waveDashDelay -= 0.5f;
        attackTime *= 1.5f;
    }

    private void undoBeserk()
    {
        moveSpeed /= 2f;
        fireRate *= 1.5f;
        jumpHeight -= 3;
        wallJumpHeight -= 3;
        maxJumpCount -= 3;
        maxWallJumpCount -= 2;
        waveDashSpeed /= 2;
        waveDashDuration -= 0.2f;
        waveDashDelay += 0.5f;
        attackTime /= 1.5f;
    }
}
