using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : AI {

    private GameObject pathFinderObject;
    private GameObject Health;
    private PathFinding pathFinding;
    private Vector3 lastUnitCoords;
    private Vector2 lastVelosityUnitVector;
    List<Vector2> path;
    private int currentPathNode;
    private double lastPlayerXcoord;
    private bool pathIsSet = false;
    private bool endWasReached = false;
    private int[,] tileMap;
    private Vector2 tileSize;
    private LevelGenerator levelGenerator;
    private GameObject levelGeneratorObject;
    private int updatesFromLastNode;
    private int updatesFromLastDirectionChange;
    private Health playerHealth;
    private double lastHitTime;

    void Start()
	{
		if (player == null) {
			player = FindObjectOfType<Player> ();
        }

        playerHealth = player.GetComponent<Health>();
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
        updatesFromLastNode = 0;
        lastHitTime = Time.realtimeSinceStartup;
		health = 2;
    }

	void Update(){
		attack();
		base.Update ();
	}
	// Update is called once per frame


	protected override void move(){
        updatesFromLastNode++;
        //Check if we need to compute path
        if (!pathIsSet || 0.1 < Math.Abs(lastPlayerXcoord - player.transform.position.x) || path.Count==0)
        {
            path = pathFinding.GoFromTo(  gameObject.transform.position.x, gameObject.transform.position.y,player.transform.position.x, player.transform.position.y);
            lastPlayerXcoord = player.transform.position.x;
            pathIsSet = true;
            endWasReached = false;
            currentPathNode = 0;
        }

        //Check if AI reached target by computing distance between
        Vector2 distnceToDestination = new Vector2();
        if (path.Count!=0)
        {
            distnceToDestination = (new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) - path[path.Count - 1]);
        }
        
        //Assign velocity based on path and currentNode (node of path struct)
        if (path.Count > 0 && 0.1 < distnceToDestination.sqrMagnitude)
        {
            //cc.enabled = false;
            Vector2 difference = path[currentPathNode] - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            Vector2 velocity = new Vector2(path[currentPathNode].x, path[currentPathNode].y) - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            velocity.Normalize();
            if (difference.sqrMagnitude < 0.1)
            {
                currentPathNode++;
                updatesFromLastNode=0;
                //Introduces delay between jumps
                if (velocity.x != lastVelosityUnitVector.x || velocity.y != lastVelosityUnitVector.y)
                {
                    rb.velocity = new Vector2(0, 0);
                }
            }
            
            rb.velocity = velocity * moveSpeed;
            if(velocity.y < 0.1)
            {
                rb.velocity *= (2 + (float)updatesFromLastNode/15f);
            }
            if (velocity.y > 0.1)
            {
                rb.velocity *= (3 - (float)updatesFromLastNode/ 20f);
            }
            
        }
        else
        {
            rb.velocity = new Vector2(0,-5);
            updatesFromLastNode = 0;
            //cc.enabled = true;
        }

        //Check if AI stucked and push it in random direction so that it can move forward
        if((lastUnitCoords - gameObject.transform.position).sqrMagnitude < 0.0005)
        {
            float randX = UnityEngine.Random.Range(-5f, 5f);
            float randY = UnityEngine.Random.Range(-5f, 5f);
            updatesFromLastNode = 0;
            rb.velocity += new Vector2(randX, randY);
        }
        else
        {
            //Flip sprite according to direction
            if (rb.velocity.x < 0)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (rb.velocity.x > 0)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        
        
        //Update AI last direction
        lastVelosityUnitVector = rb.velocity.normalized;

        //Update AI last coords
        lastUnitCoords = gameObject.transform.position;

    }

	protected override void attack(){
        double currentTIme = Time.realtimeSinceStartup;
        if (currentTIme - lastHitTime > 1 && (player.transform.position - gameObject.transform.position).sqrMagnitude < 0.5)
        {
            playerHealth.takeDamage(5);
            lastHitTime = currentTIme;
        }
	}
}
