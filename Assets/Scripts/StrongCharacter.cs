using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StrongCharacter : Player {
	void Start()
	{
		moveSpeed = 5;
		jumpHeight = 8.5f;
		maxJumpCount = 2;
        walljumpCount = 4;
		waveDashSpeed = 5;
		waveDashDuration = 0.5f;
		waveDashDelay = 1.5f;
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
	}

	protected override void attack(){
		throw new UnityException ("NOT YET IMPLEMENTED");
	}

	protected override void usePower(){
		throw new UnityException ("NOT YET IMPLEMENTED");
	}
}
