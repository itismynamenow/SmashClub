using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public GameObject ground;
	public GameObject enemy1;
	public GameObject enemy2;
	public GameObject enemy3;
    public GameObject enemy4;
	private GameObject[] enemies = new GameObject[4];
    public GameObject spikes;
	public GameObject chest;
    public Sprite grass, dirt;
	public GameObject sand;
	public GameObject ice;
	public float cameraScale;
	Camera mainCam;
    public float blockSize = 0.7f;
    public float width = 0.7f, height = 0.7f; 
	public int character = 0;
	public static int score;

	public Vector2 mapTranslation;
	public GameObject border;
	private GameObject leftBorder;
	private GameObject rightBorder;
	private GameObject topBorder;
	private GameObject bottomBorder;
	public static int enemyCount;
	private int wave = 0;
	public GameObject player;
	static string[] lastPowerups = new string[2];

	private enum drops{
		health,
		invuln,
		fireRate,

	};

    // 1 = ground, 2 = player, 3 = enemy, 4 = spikes, 5 =
	public int[,] tileMap = new int[24,42] {
        { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 },
        { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,2,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1 },
        { 1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1 },
        { 1,1,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1 },
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
        { 0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1 },
        { 1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1 },
        { 1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1 },
        { 1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
        { 0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,4,4,0,0,0,0,0,0,0,0,4,4,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 }
    };

	public int[,] terrainMap = new int[24,42] {
        { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 },
        { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,2,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1 },
        { 1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1 },
        { 1,1,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1 },
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
        { 0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1 },
        { 1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1 },
        { 1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1 },
        { 1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
        { 0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,4,4,0,0,0,0,0,0,0,0,4,4,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,5,6,6,6,6,6,6,5,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 }
    };

    void Awake()
    {
        //grass = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Art/grassMid.png", typeof(Sprite)); 
        //dirt = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Art/grassCenter.png", typeof(Sprite));
    }

    // Use this for initialization
    void Start () {
		score = 0;
		//Debug.Log("tileMap size " + tileMap.GetLength(0) + "," + tileMap.GetLength(1));
        //ground = GameObject.Find("ground");
        //enemy = GameObject.Find("enemy");
        character = Globals.getCharacter();
        Debug.Log("Character = " + character);
		if (character == 0) {
			player = (GameObject)Resources.Load("Prefabs/speed");
		} else if (character == 1)
        {
            player = (GameObject)Resources.Load("Prefabs/shoot");
        }
        else if (character == 2)
        {
            player = (GameObject)Resources.Load("Prefabs/feisty");
        }

        ground.GetComponent<SpriteRenderer>().sprite = grass;
        BoxCollider2D collider= ground.GetComponent<BoxCollider2D>();
        Vector2 size = collider.size;      

		// instantiate borders for teleporting
		leftBorder = Instantiate (border, new Vector3((mapTranslation.x-blockSize),((tileMap.GetLength(0)+1)*height)/2,0), Quaternion.identity);
		leftBorder.transform.localScale = new Vector3 (leftBorder.transform.localScale.x, tileMap.GetLength(0) + mapTranslation.y, 0);
		leftBorder.tag = "left";
	
		rightBorder = Instantiate (border, new Vector3 ((mapTranslation.x + width * tileMap.GetLength (1)), ((tileMap.GetLength (0) + 1) * height) / 2, 0), Quaternion.identity);
		rightBorder.transform.localScale = new Vector3 (rightBorder.transform.localScale.x, tileMap.GetLength(0) + mapTranslation.y, 0);
		rightBorder.tag = "right";

		bottomBorder = Instantiate (border, new Vector3 (((tileMap.GetLength (1)-1) * width) / 2, 0, 0), Quaternion.identity);
		bottomBorder.transform.localScale = new Vector3 (tileMap.GetLength (1) + mapTranslation.x, bottomBorder.transform.localScale.y, 0);
		bottomBorder.tag = "bottom";

		topBorder = Instantiate (border, new Vector3 (((tileMap.GetLength (1) - 1) * width) / 2, (tileMap.GetLength(0)+1) * height, 0), Quaternion.identity);
		topBorder.transform.localScale = new Vector3 (tileMap.GetLength (1) + mapTranslation.x, bottomBorder.transform.localScale.y, 0);
		topBorder.tag = "top";

		for (int x = 0; x < tileMap.GetLength(1); x++)
        {
            for (int y = 0; y < tileMap.GetLength(0); y++)
            { 
                if(tileMap[y,x] > 0)
                {
                    float xTilePosition =  (x * width + mapTranslation.x);
                    float yTilePosition = (tileMap.GetLength(0) - y) * height + mapTranslation.y;
                    if(y > 0)
                    {
                        if(tileMap[y-1,x] == 1)
                        {
                            ground.GetComponent<SpriteRenderer>().sprite = dirt;
                        }
                    }
					if (tileMap [y, x] == 3) {
						Instantiate (enemy2, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
					} else if (tileMap [y, x] == 4) {
						Instantiate (spikes, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
					} else if (tileMap [y, x] == 1) {
                        GameObject terrain;
						switch (terrainMap [y, x]) {
						case 1:
							terrain = Instantiate (ground, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
                                terrain.name = "[" + y + ", " + x + "]";
							break;
						case 5:
                                terrain = Instantiate(sand, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
                                terrain.name = "[" + y + ", " + x + "]";
                                break;
						case 6:
                                terrain = Instantiate(ice, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
                                terrain.name = "[" + y + ", " + x + "]";
                                break;
						}
					} else if (tileMap [y, x] == 2) {
						Instantiate (player, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
					} else if (tileMap [y, x] == 5) {
						Instantiate (enemy4, new Vector3 (xTilePosition, yTilePosition, 0), Quaternion.identity);
					}
                    ground.GetComponent<SpriteRenderer>().sprite = grass;
                }
            }
        }
		mainCam = Camera.main;
		mainCam.transform.position = new Vector3 ((((tileMap.GetLength (1) - 1) * width) / 2), ((tileMap.GetLength (0)) * height) / 2, -1);
		mainCam.orthographicSize = cameraScale;
		enemies [0] = enemy1;
		enemies [1] = enemy2;
		enemies [2] = enemy3;
        enemies [3] = enemy4;
		lastPowerups [0] = "";
		lastPowerups [1] = "";
		enemyCount = 0;
	}

    void Update()
    {
		ScoreCounter.scoreCount = score;
        if (enemyCount <= 0)
        {
			spawnWave (++wave);
        }
    }
    public void randomEnemySpawn()
    {
        int randX, randY, randEnemy;
		GameObject enemy;
        do
        {
            randX = Random.Range(3, tileMap.GetLength(1) - 3);
            randY = Random.Range(3, tileMap.GetLength(0) - 3);
			randEnemy = Random.Range(0,enemies.Length);
		} while (!(tileMap[randY, randX] == 0 && tileMap[randY + 1, randX] == 1));

		enemy = enemies [randEnemy];

        float xTilePosition = (randX * width + mapTranslation.x);
		float yTilePosition = (((tileMap.GetLength(0) - randY)* height) + mapTranslation.y);
        if(enemy.transform.name.Equals("Golem"))
        {
            Instantiate(enemy, GolemAI.getSpawnLocation(randY, randX, tileMap.GetLength(0)), Quaternion.identity);
        }
        else
        {
            Instantiate(enemy, new Vector3(xTilePosition, yTilePosition, 0), Quaternion.identity);
        }
		enemyCount++;
    }

	private void spawnWave(int wave){
		int pickups;

		int enemiesToSpawn = (1 << wave);
		if (enemiesToSpawn > 16) {
			enemiesToSpawn = 16;
		}

		if (wave > 4) {
			pickups = 4;
		} else {
			pickups = wave;
		}

		for (int i = 0; i < pickups; ++i) {
			spawnPickup ();
		}

		for (int i = 1; i <= enemiesToSpawn; ++i) {
			randomEnemySpawn ();
		}
	}

	private void spawnPickup(){
		int randX, randY;

		do
		{
			randX = Random.Range(0, tileMap.GetLength(1) - 1);
			randY = Random.Range(0, tileMap.GetLength(0) - 1);
		} while (tileMap[randY, randX] != 0 || tileMap[randY + 1, randX] != 1);
			
		float xTilePosition = (randX * width + mapTranslation.x);
		float yTilePosition = (((tileMap.GetLength(0) - randY)* height) + mapTranslation.y);
		Instantiate(chest, new Vector3(xTilePosition, yTilePosition, 0), Quaternion.identity);
	}
	public string getDrop(){
		
		drops powerup = (drops)Random.Range (0, drops.GetValues (typeof(drops)).Length);
		while (powerup.ToString() == lastPowerups[1] && powerup.ToString() == lastPowerups [0]) {
			powerup = (drops)Random.Range (0, drops.GetValues (typeof(drops)).Length);
		}

		lastPowerups [1] = lastPowerups [0];
		lastPowerups [0] = powerup.ToString();
		Debug.Log (lastPowerups[0] + " " + lastPowerups[1]);
		return powerup.ToString ();
	}
}
