 using UnityEngine;
 using System.Collections;
 
 public abstract class AI : Entity
 {
	protected static Player player;
    protected Rigidbody2D rb;
	protected RaycastHit2D isPlatLeft, isPlatRight, onGround, isWallLeft, isWallRight;
	public int direction;
    public bool wall;
    public LayerMask wallMask;
    protected CircleCollider2D cc;
	protected float sightRange;
	protected float health;
	protected static int gameMode = -1;
	public int damage;
    int prevGameMode = -1;
	protected enum states{
		wander,
		attack,
		flee
	};
	protected states currentState;
    // Use this for initialization

	protected void Start(){
		gameMode = Globals.getGameMode ();
        if(prevGameMode == -1)
        {
            switch (gameMode)
            {
                case 1:
                    health = 2 * health;
                    damage = 2 * damage;
                    break;
            }
        }
		else if(prevGameMode != gameMode)
        {
            switch (gameMode)
            {
                case 0:
                    health /= 2;
                    damage /= 2;
                    break;
                case 1:
                    health *= 2;
                    damage *= 2;
                    break;
            }
        }
        prevGameMode = gameMode;
	}
	protected override void move(KeyCode button){
		throw new UnityException ("DON'T CALL THIS WHY WOULD THE ENEMY RESPOND TO A KEYPRESS");
	}

	protected void Update () {
        move();
    }
	protected bool playerInSight(){
//		Vector3 playerDirection = gameObject.transform.position - player.transform.position;
//		RaycastHit2D checkForPlayer = Physics2D.Raycast (new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y), new Vector2 (playerDirection.x, playerDirection.y));
		return false;
	}

	public virtual void doDamage(int damage){
		health -= damage;
		if (health <= 0) {
			Destroy (gameObject);
			LevelGenerator.enemyCount--;
			LevelGenerator.score++;
		}
	}
 }