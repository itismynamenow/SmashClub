﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedCharacter : Player {
	// Shooting projectile variables
	private GameObject projectile;
	private float nextFire;
    private bool isBeserk = false;

    void Update()
    {
        if (!isBeserk && playerHealth.HealthRatio <= 0.21f)
        {
            Globals.setBeserk(true);
            isBeserk = true;
            goBeserk();
        }
        else if (isBeserk && playerHealth.HealthRatio > 0.21f)
        {
            Globals.setBeserk(false);
            isBeserk = false;
            undoBeserk();
        }
        base.Update();
    }

	void Start()
	{
		gameObject.transform.tag = "Player";
		playerHealth = GetComponent<Health> ();
		playerHealth.maxHealth = 50;
		damage = 2;
		moveSpeed = 9;
		fireRate = 0.25f;
		jumpHeight = 10;
        wallJumpHeight = 5;
		maxJumpCount = 3;
        maxWallJumpCount = 6;
		waveDashSpeed = 7;
		waveDashDuration = 0.3f;
		waveDashDelay = 1.2f;

        

        projectile = (GameObject)Resources.Load("Prefabs/blueProjectile");

		rb = GetComponent<Rigidbody2D>();
		tr = GetComponent<Transform>();
		anim = GetComponent<Animator> ();

		levelMap = GameObject.Find("LevelGenerator");
		lg = levelMap.GetComponent<LevelGenerator>();
		for (int x = 0; x < lg.tileMap.GetLength(1); x++)
		{
			for (int y = 0; y < lg.tileMap.GetLength(0); y++)
			{
				if(lg.tileMap[y,x] == 2)
				{
					float xStartPosition = (x * lg.blockSize) + lg.mapTranslation.x;
					float yStartPosition = (lg.tileMap.GetLength(0) - y) * lg.blockSize + lg.mapTranslation.y;
					tr.position = new Vector3(xStartPosition, yStartPosition, 0);
				}
			}
		}
		base.Start ();
	}

	protected override void attack(){
        // Checks if delay is active
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
            ProjectileController missile = proj.GetComponent<ProjectileController>();

            Vector3 dir = mousePos - transform.position;
            dir = transform.InverseTransformDirection(dir);
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            missile.transform.Rotate(new Vector3(0, 0, angle));
            missile.setAngle(angle);
            CooldownController.Cooldown3();
        }
    }

	protected override void usePower(){
		return;
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
        moveSpeed *= 1.5f;
        fireRate /= 1.5f;
        jumpHeight += 3;
        wallJumpHeight += 3;
        maxJumpCount += 3;
        maxWallJumpCount += 2;
        waveDashSpeed *= 2;
        waveDashDuration += 0.2f;
        waveDashDelay -= 0.5f;
    }

    private void undoBeserk()
    {
        moveSpeed /= 1.5f;
        fireRate *= 1.5f;
        jumpHeight -= 3;
        wallJumpHeight -= 3;
        maxJumpCount -= 3;
        maxWallJumpCount -= 2;
        waveDashSpeed /= 2;
        waveDashDuration -= 0.2f;
        waveDashDelay += 0.5f;
    }
}