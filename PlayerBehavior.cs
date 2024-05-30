using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerBehavior : MonoBehaviour
{
	public GameOverScreen gameOverScreen;
	public int currentHealth;
	public int startingHealth = 100;
	public HealthBar healthBar;
	public AudioSource hurt;
	public GameObject health;
	void Start()
	{
		currentHealth = startingHealth;
		health.GetComponent<TextMeshProUGUI>().text = "HP:" + currentHealth;
		healthBar.setHealth(currentHealth, startingHealth);
	}

	void Update()
	{
        healthBar.setHealth(currentHealth, startingHealth);
		

    }

        void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Water"))
		{
			hurt.Play();
			TakeDamage(currentHealth);
			health.GetComponent<TextMeshProUGUI>().text = "HP:" + currentHealth;
			
		}

		if (other.CompareTag("EnemyBullet"))
		{
			hurt.Play();
			Destroy(other.gameObject); // destroy the bullet
			TakeDamage(25);

			health.GetComponent<TextMeshProUGUI>().text = "HP:" + currentHealth;

		}

		if (currentHealth <= 0)
		{
			EndGame();
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("EnemyZombie"))
		{
			EndGame();
		}

	}

	void TakeDamage(int damage)
	{
		currentHealth -= damage;
		healthBar.setHealth(currentHealth, startingHealth);
	}

	void EndGame()
	{
		//show the game over screen
		gameOverScreen.Setup();

		//disable player controls
		GetComponent<PlayerBehavior>().enabled = false;

		//stop all audio sources
		AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
		foreach (AudioSource audioSource in allAudioSources)
		{
			audioSource.Stop(); 
		}
	}

	public void IncreaseHealth(){
		currentHealth += 25;
		health.GetComponent<TextMeshProUGUI>().text = "HP:" + currentHealth;
	}
}

