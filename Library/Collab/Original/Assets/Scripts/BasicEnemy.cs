using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : AI {

    private GameObject pathFinderObject;
    private PathFinding pathFinding;
    private Vector3 lastUnitCoords;
    List<Vector2> path;
    int currentPathNode;
    double lastPlayerXcoord;
    bool pathIsSet = false;
    bool endWasReached = false;
    private int[,] tileMap;
    private Vector2 tileSize;
    private LevelGenerator levelGenerator;
    private GameObject levelGeneratorObject;

    void Start()
	{
		if (player == null) {
			player = FindObjectOfType<Player> ();
        }


        pathFinderObject = GameObject.Find("PathFinder");
        levelGeneratorObject = GameObject.Find("LevelGenerator");
        levelGenerator = levelGeneratorObject.GetComponent<LevelGenerator>();
        pathFinding = pathFinderObject.GetComponent<PathFinding>();
		rb = GetComponent<Rigidbody2D> ();
		cc = GetComponent<CircleCollider2D> ();
		direction = 1;
		moveSpeed = 3f;
		currentState = states.wander;
        tileSize.y = levelGenerator.GetComponent<LevelGenerator>().height;
        tileSize.x = levelGenerator.GetComponent<LevelGenerator>().width;
        tileMap = levelGenerator.GetComponent<LevelGenerator>().tileMap;

    }
	
	// Update is called once per frame


	protected override void move(){
        int rowStart = tileMap.GetLength(0) - (int)((gameObject.transform.position.y) / tileSize.y); //-1 * ((tileMap.GetLength(1) - randX) * width + mapTranslation.x);
        int columnStart = (int)((gameObject.transform.position.x) / tileSize.x);
        //if(tileMap[rowStart,columnStart]==1)
        //{
        //    //Velocity stays same
        //    return;
        //}

        if (!pathIsSet || 0.1 < Math.Abs(lastPlayerXcoord - player.transform.position.x) || path.Count==0)
        {
            path = pathFinding.GoFromTo(  gameObject.transform.position.x+0.5f, gameObject.transform.position.y,player.transform.position.x, player.transform.position.y);
            lastPlayerXcoord = player.transform.position.x;
            pathIsSet = true;
            endWasReached = false;
            currentPathNode = 0;
        }
        //for (int i = 1; i < path.Count; i++)
        //{
        //    Debug.DrawLine(new Vector3(path[i].x, path[i].y, 0),
        //                   new Vector3(path[i - 1].x, path[i - 1].y, 0),
        //                           Color.white, 1, false);
        //}

        Vector2 distnceToDestination = new Vector2();
        if (path.Count!=0)
        {
            distnceToDestination = (new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) - path[path.Count - 1]);
        }
        
        if (path.Count > 0 && 0.1 < distnceToDestination.sqrMagnitude)
        {
            cc.enabled = false;
            Vector2 difference = path[currentPathNode] - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            if (difference.sqrMagnitude < 0.1)
            {
                currentPathNode++;
            }
            Vector2 velocity = path[currentPathNode] - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            velocity.Normalize();
            rb.velocity = velocity * 8;
            //Debug.DrawLine(new Vector3(path[currentPathNode].x, path[currentPathNode].y, 0),
            //               new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0),
            //                       Color.magenta, 10, false);
            //Debug.DrawLine(new Vector3(gameObject.transform.position.x + velocity.x, path[currentPathNode].y + velocity.y, 0),
            //               new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0),
            //                       Color.green, 10, false);
        }
        else
        {
            rb.velocity = new Vector2(0,-5);
            //cc.enabled = true;
        }
        lastUnitCoords = gameObject.transform.position;

    }

	protected override void attack(){

	}
}
