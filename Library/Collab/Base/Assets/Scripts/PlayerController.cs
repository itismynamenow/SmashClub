using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float stopSpeed;
    public float jumpHeight;
    public int maxJumpCount;
    public float waveDashSpeed;
    public float waveDashDuration;
    public float waveDashDelay;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    private bool facingRight;
    private bool grounded;
    private bool waveDashed;
    private bool airWaveDashed;
    private int jumpCount;

    private float waveDashDurationTimer;
    private float waveDashDelayTimer;
    private float addWaveSpeed = 0.0f;

    private GameObject levelMap;
    private LevelGenerator lg;

    private Rigidbody2D rb;
    private Transform tr;
    public Animator anim;
    private SpriteRenderer sr;

    // Shooting projectile variables
    public Transform projectileLocation;
    public GameObject projectile;
    public float fireRate;
    private float nextFire;

    private bool animGrounded;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        levelMap = GameObject.Find("LevelGenerator");
        lg = levelMap.GetComponent<LevelGenerator>();
        for (int x = 0; x < lg.tileMap.GetLength(1); x++)
        {
            for (int y = 0; y < lg.tileMap.GetLength(0); y++)
            {
                if(lg.tileMap[y,x] == 2)
                {
                    float xStartPosition = -1 * ((lg.tileMap.GetLength(1) - x) * lg.blockSize + lg.mapTranslation.x + lg.blockSize/2);
                    float yStartPosition = (lg.tileMap.GetLength(1) - y) * lg.blockSize + lg.mapTranslation.y;
                    tr.position = new Vector3(xStartPosition, yStartPosition, 0);
                }
            }
        }
    }

    // Recommended for physics stuff
    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

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
    }

    // Update is called once per frame
    void Update()
    { 
        animGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0, -1), 0.8f, whatIsGround);
        if(animGrounded)
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
            airWaveDashed = false;
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
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            ++jumpCount;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(moveSpeed + addWaveSpeed, rb.velocity.y);
            sr.flipX = false;
            facingRight = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            rb.velocity = new Vector2(stopSpeed, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-moveSpeed - addWaveSpeed, rb.velocity.y);
            sr.flipX = true;
            facingRight = false;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            rb.velocity = new Vector2(-stopSpeed, rb.velocity.y);
        }
        //Wavedash keypress
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.D) && !waveDashed && !airWaveDashed)
        {
            rb.velocity = new Vector2(waveDashSpeed + moveSpeed, rb.velocity.y);
            waveDashed = true;
            addWaveSpeed = waveDashSpeed;
            waveDashDurationTimer = waveDashDuration;
            waveDashDelayTimer = waveDashDelay;
            anim.SetBool("dash", true);
            facingRight = true;
            if (!grounded)
            {
                airWaveDashed = true;
            }
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.A) && !waveDashed && !airWaveDashed)
        {
            rb.velocity = new Vector2(-waveDashSpeed - moveSpeed, rb.velocity.y);
            waveDashed = true;
            addWaveSpeed = waveDashSpeed;
            waveDashDurationTimer = waveDashDuration;
            waveDashDelayTimer = waveDashDelay;
            anim.SetBool("dash", true);
            facingRight = false;
            if (!grounded)
            {
                airWaveDashed = true;
            }
        }

        // Shoot is 'L' button
        if(Input.GetKeyDown(KeyCode.L))
        {
            shoot();
        }
    }

    void shoot()
    {
        // Checks if delay is active
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (facingRight)
            {
                Instantiate(projectile, projectileLocation.position, Quaternion.Euler(new Vector3(0, 0, 180)));
            }
            else // flips the projectile 180*
            {
                Instantiate(projectile, projectileLocation.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            }
        }
    }
}
