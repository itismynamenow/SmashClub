using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float invincibilityDuration;
    private float invincibilityTimer;
    public bool invincible;

    public Image HealthBG;
    public Image HealthBar;
    public Text HealthText;
    public float HealthRatio;
	private static Player player = null;
	private IEnumerator coroutine;
	private float parryInvuln = 0.2f;

    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;
    public AudioClip hurtSound;


    private void Start()
    {
        source = GetComponent<AudioSource>();

        if (player == null) {
			player = GetComponent<Player> ();
		}
        HealthBG = GameObject.Find("Canvas").GetComponentInChildren<Transform>().GetComponentInChildren<Image>();
        HealthBar = HealthBG.transform.Find("HealthBar").gameObject.GetComponentInChildren<Image>();
        HealthText = HealthBG.transform.Find("HealthText").gameObject.GetComponentInChildren<Text>();
        currentHealth = maxHealth;
        invincible = false;
		HealthRatio = currentHealth / maxHealth;
        HealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    void Update()
    {
        //invincibility timer
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (invincibilityTimer <= 0)
        {
            invincible = false;
        }

        HealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    //health system
	public void takeDamage(int damage)
    {
        if (!invincible)
        {
            currentHealth -= damage;
			if (currentHealth <= 0) {
				currentHealth = 0;
            }

			HealthRatio = currentHealth/maxHealth;
            HealthBar.rectTransform.localScale = new Vector3(HealthRatio, 1, 1);

			coroutine = playerFlash ();
			StartCoroutine (coroutine);
            float vol = UnityEngine.Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(hurtSound, vol);

            if (currentHealth == 0) {
                Globals.setScore (ScoreCounter.scoreCount);

                Debug.Log("Game Ended. Gamemode was " + Globals.getGameMode());
                
                if (PlayerPrefs.GetInt("High Score Normal") < ScoreCounter.scoreCount && Globals.getGameMode() == 0)
                {
                    Debug.Log("New Highscore");
                    PlayerPrefs.SetInt("High Score Normal", ScoreCounter.scoreCount);
                }
                else if (PlayerPrefs.GetInt("High Score Hard") < ScoreCounter.scoreCount && Globals.getGameMode() == 1)
                {
                    Debug.Log("New Highscore");
                    PlayerPrefs.SetInt("High Score Hard", ScoreCounter.scoreCount);
                }
                SceneManager.LoadScene("GameOver");


                SceneManager.LoadScene("GameOver");
            }				
            invincibilityTimer = invincibilityDuration;
            invincible = true;
        }

    }

    public void parry()
    {
        coroutine = parryFlash();
        StartCoroutine(coroutine);
        invincibilityTimer = parryInvuln;
        invincible = true;
    }

    public void healDamage(float x)
    {
		if (currentHealth < maxHealth)
        {
            currentHealth += x;
			if(currentHealth > maxHealth) { currentHealth = maxHealth; }
			HealthRatio = currentHealth / maxHealth;
            HealthBar.rectTransform.localScale = new Vector3(HealthRatio, 1, 1);
        }
    }

	public float getHealth(){
		return currentHealth;
	}

	public void setInvincible(bool invuln, float time){
		this.invincible = invuln;
		this.invincibilityTimer = time;
	}

	IEnumerator playerFlash(){
		Renderer renderer = player.GetComponent<Renderer> ();
		int count = (int)(invincibilityDuration / 0.2f);
		for(var n = 0; n < count; n++)
		{
			renderer.enabled = true;
			yield return new WaitForSeconds(.1f);
			renderer.enabled = false;
			yield return new WaitForSeconds(.1f);
		}
		renderer.enabled = true;
	}

    IEnumerator parryFlash()
    {
        Renderer renderer = player.GetComponent<Renderer>();
        int count = 1;
        for (var n = 0; n < count; n++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(.1f);
			renderer.enabled = true;
			yield return new WaitForSeconds(.1f);
        }
        renderer.enabled = true;
    }
}