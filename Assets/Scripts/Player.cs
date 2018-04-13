using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

abstract public class Player : Entity
{
    protected float jumpHeight;
    protected float wallJumpHeight;
    protected int maxJumpCount;
    protected int maxWallJumpCount;
    protected float waveDashSpeed;
    protected float waveDashDuration;
    protected float waveDashDelay;
    protected float fireRate;
    public int damage;
    private float powerupTime = 5f;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    public Transform RwallCheck;
    public float RwallTouchCheckRadius;
    public LayerMask whatIsRWall;

    public Transform LwallCheck;
    public float LwallTouchCheckRadius;
    public LayerMask whatIsLWall;

    protected bool facingRight = true;
    protected bool grounded;
    protected bool touchingRWall;
    protected bool touchingLWall;
    protected bool waveDashed;
    protected bool airWaveDashed;
    protected int jumpCount;
    protected int walljumpCount;
    protected Vector3 mousePos;

    protected float waveDashDurationTimer;
    protected float waveDashDelayTimer;
    protected float addWaveSpeed = 0.0f;

    protected GameObject levelMap;
    protected LevelGenerator lg;

    protected Rigidbody2D rb;
    protected Transform tr;
    public Animator anim;
    protected SpriteRenderer sr;
    protected bool animGrounded;
    protected Health playerHealth;
    protected static int gameMode = -1;
    private float parryTimer = 2.0f;
    private bool canParry = true;

    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;
    public AudioClip jumpSound;
    public AudioClip runSound;
    public AudioClip attackSound;
    public AudioClip dashSound;
    public AudioClip powerUpSound;

    private bool lockSound = false;
    private float fireRateCountdown;
    private int prevGameMode = -1;
    protected enum terrain
    {
        ground,
        sand,
        ice
    };

    protected terrain currentTerrain;
    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        touchingRWall = Physics2D.OverlapCircle(RwallCheck.position, RwallTouchCheckRadius, whatIsRWall);
        touchingLWall = Physics2D.OverlapCircle(LwallCheck.position, LwallTouchCheckRadius, whatIsLWall);

        if (waveDashDurationTimer > 0)
        {
            waveDashDurationTimer -= Time.deltaTime;
        }
        else if (waveDashDurationTimer <= 0)
        {
            addWaveSpeed = 0.0f;
        }

        if (waveDashDelayTimer > 0)
        {
            waveDashDelayTimer -= Time.deltaTime;
        }
        else if (waveDashDelayTimer <= 0)
        {
            waveDashed = false;
            anim.SetBool("dash", false);
        }

        if (!canParry)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0)
            {
                canParry = true;
            }
        }
    }
    protected void Start()
    {
        Debug.Log("base start called");        

        gameMode = Globals.getGameMode();
//        if (prevGameMode == -1)
//        {
//            switch (gameMode)
//            {
//                case 1:
//                    playerHealth.currentHealth = playerHealth.maxHealth;
//                    damage *= (int)damage / 1.5f;
//                    break;
//            }
//        }
//        else if (prevGameMode != gameMode)
//        {
//            switch (gameMode)
//            {
//                case 0:
//                    playerHealth.currentHealth = playerHealth.maxHealth;
//                    damage *= (int)damage * 1.5f;
//                    break;
//                case 1:
//                    playerHealth.currentHealth = playerHealth.maxHealth;
//                    damage *= (int)damage / 1.5f;
//                    break;
//            }
//        }

        prevGameMode = gameMode;

        source = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    protected void Update()
    {
        fireRateCountdown -= Time.deltaTime;

        animGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0, -1), 0.8f, whatIsGround);

        if (animGrounded)
        {
            anim.SetBool("grounded", true);
        }
        else
        {
            anim.SetBool("grounded", false);
        }

        anim.SetBool("dash", false);

        sr = GetComponent<SpriteRenderer>();
        if (grounded)
        {
            jumpCount = 0;
            walljumpCount = 0;
            airWaveDashed = false;
        }

        if (touchingRWall || touchingLWall)
        {
            grounded = false;
        }

        if (!grounded && jumpCount == 0)
        {
            // Fixes first jump not getting detected
            ++jumpCount;
        }

        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        //Basic WASD keypress 
        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumpCount)
        {
            move(KeyCode.W);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(jumpSound, vol);
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            move(KeyCode.D);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(runSound, vol);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            rb.velocity = new Vector2(stopSpeed, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            move(KeyCode.A);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(runSound, vol);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            rb.velocity = new Vector2(-stopSpeed, rb.velocity.y);
        }

        //Wavedash keypress
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.D) && !waveDashed && !airWaveDashed)
        {
            dash(KeyCode.D);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(dashSound, vol);
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.A) && !waveDashed && !airWaveDashed)
        {
            dash(KeyCode.A);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(dashSound, vol);
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.W) && !waveDashed && !airWaveDashed)
        {
            dash(KeyCode.W);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(dashSound, vol);
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.S) && !waveDashed && !airWaveDashed)
        {
            dash(KeyCode.S);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(dashSound, vol);
        }

        //Wall jumping
        if (Input.GetKeyDown(KeyCode.LeftShift) && touchingRWall && !grounded && walljumpCount < maxWallJumpCount)
        {
            RWallJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && touchingLWall && !grounded && walljumpCount < maxWallJumpCount)
        {
            LWallJump();
        }

        // Shoot is 'Left Click'
        if (Input.GetMouseButton(0))
        {
            mousePos = Input.mousePosition;
            mousePos.z = 0;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.Log(mousePos);
            attack();
            mousePos = Input.mousePosition;

            //Plays sound only once
            if(!lockSound && fireRateCountdown <= 0)
            {
                float vol = Random.Range(volLowRange, volHighRange);
                source.PlayOneShot(attackSound, vol);
                fireRateCountdown = fireRate;
            }
            lockSound = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            lockSound = false;
        }

        // Parry is 'Right Click'
        if (Input.GetMouseButton(1))
        {
            if (canParry)
            {
                GetComponent<Health>().parry();
                parryTimer = 2.0f;
                canParry = false;
                CooldownController.Cooldown2();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            usePower();
        }
    }
    void RWallJump()
    {
        rb.velocity = new Vector2((-wallJumpHeight - moveSpeed) / 2, jumpHeight / 2);
        walljumpCount++;
        float vol = Random.Range(volLowRange, volHighRange);
        source.PlayOneShot(jumpSound, vol);
    }

    void LWallJump()
    {
        rb.velocity = new Vector2((wallJumpHeight + moveSpeed) / 2, jumpHeight / 2);
        walljumpCount++;
        float vol = Random.Range(volLowRange, volHighRange);
        source.PlayOneShot(jumpSound, vol);
    }


    protected override void move(KeyCode button)
    {
        switch (button)
        {
            case KeyCode.W:
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                ++jumpCount;
                break;
            case KeyCode.D:
                rb.velocity = new Vector2(moveSpeed + addWaveSpeed, rb.velocity.y);
                sr.flipX = false;
                facingRight = true;
                break;
            case KeyCode.A:
                rb.velocity = new Vector2(-moveSpeed - addWaveSpeed, rb.velocity.y);
                sr.flipX = true;
                facingRight = false;
                break;
        }

        switch (currentTerrain)
        {
            case terrain.sand:
                rb.velocity = new Vector2(rb.velocity.x - 0.5f * rb.velocity.x, rb.velocity.y);
                break;
            case terrain.ice:
                Debug.Log(currentTerrain.ToString());
                rb.velocity = new Vector2(rb.velocity.x + 0.5f * rb.velocity.x, rb.velocity.y);
                break;
        }
    }

    protected void dash(KeyCode button)
    {
        switch (button)
        {
            case KeyCode.D:
                rb.velocity = new Vector2(waveDashSpeed + moveSpeed, rb.velocity.y);
                break;
            case KeyCode.A:
                rb.velocity = new Vector2(-waveDashSpeed - moveSpeed, rb.velocity.y);
                break;
            case KeyCode.W:
                rb.velocity = new Vector2(rb.velocity.x, waveDashSpeed + moveSpeed);
                break;
            case KeyCode.S:
                rb.velocity = new Vector2(rb.velocity.x, -waveDashSpeed - moveSpeed);
                break;
        }
        CooldownController.Cooldown1();
        waveDashed = true;
        addWaveSpeed = waveDashSpeed;
        waveDashDurationTimer = waveDashDuration;
        waveDashDelayTimer = waveDashDelay;
        anim.SetBool("dash", true);

        if (!grounded)
        {
            airWaveDashed = true;
        }
    }

    protected override void move()
    {
        throw new UnityException("NOT IMPLEMENTED WITHOUT PARAMETER");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "chest")
        {
            string powerup = lg.getDrop();
            Destroy(other.gameObject);
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(powerUpSound, vol);
            IEnumerator coroutine = setPowerup(powerup);
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
        if (other.tag == "explosion")
        {
            Explosion explosion = other.GetComponent<Explosion>();
            playerHealth.takeDamage(explosion.damage);
        }
    }


    protected IEnumerator setPowerup(string powerup)
    {
		if (fireRate == 0) {
			yield return new WaitForEndOfFrame ();
		}
		else{
			float rate = fireRate;

	        switch (powerup)
	        {
	            case ("fireRate"):
	                fireRate = 0;
	                Pickup.powerup = "GO SHOOTING";
	                yield return new WaitForSeconds(powerupTime);
	                Pickup.powerup = "";
	                fireRate = rate;
	                break;
	            case ("health"):
	                Pickup.powerup = "RECOVERY SEASON";
	                playerHealth.healDamage(playerHealth.maxHealth / 4);
	                yield return new WaitForSeconds(powerupTime);
	                Pickup.powerup = "";
	                break;
	            case ("invuln"):
	                playerHealth.setInvincible(true, powerupTime);
	                Pickup.powerup = "GOD MODE";
	                yield return new WaitForSeconds(powerupTime);
	                playerHealth.setInvincible(false, 0);
	                Pickup.powerup = "";
	                break;
	        }
		}
    }
    protected abstract void usePower();
    public float getParryCool()
    {
        return parryTimer;
    }
}
