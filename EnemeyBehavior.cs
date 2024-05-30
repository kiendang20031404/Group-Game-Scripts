using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    private bool facingRight;
    private int currentHealth;
    private float flashDuration = 0.1f;
    private Color flashColor = Color.red; // Color of the flash
    private Color originalColor; // Original color of the enemy
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private bool isHit = false;
    private Transform player; // Reference to the player's transform
    public float followSpeed = 5f; // Speed at which the enemy follows the player
    public float activationDistance = 5f; // Distance at which following behavior is activated

    private bool isShooting;

    private Animator anim;

    private float shootTimer = 0f;


    [Header("Enemy Properties")]

    public HealthBar healthBar;
    public int startingHealth;
    public GameObject bullet;  // bullet sprite
    public GameObject gunPoint;  // the position where the bullet fires
    private Rigidbody2D rigid;  // rigidbody for recoil tuning
    private Transform sprite;


    [Header("shooting Speed")]
    public float shootDelay = 1f;

    [Header("Bullet Velocity")]
    public float bulletVelocity = 125f;

    [Header(" bullet destroy time")]

    public float destroyDelay = 1f;

    [Header("Enemy Mode")]
    public bool chasing = false;
    public bool stationaryShooting = false;

    public bool runAndGun = false;
    void Start()
    {
        currentHealth = startingHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        rigid = GetComponent<Rigidbody2D>();

        sprite = GetComponent<Transform>();

        originalColor = spriteRenderer.color;
        healthBar.setHealth(currentHealth, startingHealth);

        player = GameObject.Find("Scout").transform;

        anim = GetComponent<Animator>();

        isShooting = false;
        facingRight = true;
    }

    void Update()
    {
        anim.SetFloat("Speed", Mathf.Abs(rigid.velocity.x));
        anim.SetBool("touchingGround", true);
        if (isHit)
        {
            Invoke("ResetHitFlag", 0.5f);
        }

        RunAndGun(runAndGun);
        Chasing(chasing);
        StationaryShooting(stationaryShooting);

        // Update the shoot timer
        shootTimer += Time.deltaTime;

        // Check if enough time has passed to shoot
        if (shootTimer >= shootDelay)
        {
            // Call the shoot method
            EnemyShoot(isShooting);

            // Reset the shoot timer
            shootTimer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(20); // Decrease health by 20 when hit by a bullet]
            healthBar.setHealth(currentHealth, startingHealth);

            // Flash the enemy in red when hit
            FlashColor();

            if (currentHealth <= 0)
            {
                Destroy(gameObject); // Destroy the enemy if health is zero or below
            }
        }

        // if it touches water, it also dies
        if (other.CompareTag("Water"))
        {
            Destroy(gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    void FlashColor()
    {
        spriteRenderer.color = flashColor;
        Invoke("ResetColor", flashDuration);
    }

    void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }

    void ResetHitFlag()
    {
        isHit = false;
    }

    void RunAndGun(bool runAndGun)
    {
        if (!runAndGun)
        {
            return;
        }
        // If player is still alive
        if (player != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the distance is within the activation range
            if (distanceToPlayer <= activationDistance)
            {
                isShooting = true;
                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = player.position - transform.position;

                // Normalize the direction vector
                directionToPlayer.Normalize();

                // Check the sign of the direction's x-component to determine the player's position relative to the enemy
                if (directionToPlayer.x < 0)
                {
                    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * -1, sprite.localScale.y, sprite.localScale.z);
                    // If the player is on the left side, move the enemy left
                    rigid.velocity = new Vector2(-followSpeed, rigid.velocity.y);

                    facingRight = false;
                }
                else
                {
                    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x), sprite.localScale.y, sprite.localScale.z);
                    // If the player is on the right side, move the enemy right
                    rigid.velocity = new Vector2(followSpeed, rigid.velocity.y);
                    facingRight = true;
                }
            }
            else
            {
                // If the player is not within the activation range, stop chasing
                isShooting = false;
            }
        }
    }

    void Chasing(bool chasing)
    {
        if (!chasing)
        {
            return;
        }
        // If player is still alive and chasing Mode is set
        if (player != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the distance is within the activation range
            if (distanceToPlayer <= activationDistance)
            {
                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = player.position - transform.position;

                // Normalize the direction vector
                directionToPlayer.Normalize();

                // Check the sign of the direction's x-component to determine the player's position relative to the enemy
                if (directionToPlayer.x < 0)
                {
                    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * -1, sprite.localScale.y, sprite.localScale.z);
                    // If the player is on the left side, move the enemy left
                    rigid.velocity = new Vector2(-followSpeed, rigid.velocity.y);

                    facingRight = false;
                }
                else
                {
                    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x), sprite.localScale.y, sprite.localScale.z);
                    // If the player is on the right side, move the enemy right
                    rigid.velocity = new Vector2(followSpeed, rigid.velocity.y);
                    facingRight = true;
                }
            }
        }
    }

    void StationaryShooting(bool StationaryShooting)
    {

        if (!StationaryShooting)
        {
            return;
        }
        // If player is still alive and stationaryShooting is Set
        if (player != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the distance is within the activation range
            if (distanceToPlayer <= activationDistance)
            {
                isShooting = true;
                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = player.position - transform.position;

                // Normalize the direction vector
                directionToPlayer.Normalize();

                // Check the sign of the direction's x-component to determine the player's position relative to the enemy
                if (directionToPlayer.x < 0)
                {
                    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * -1, sprite.localScale.y, sprite.localScale.z);

                    facingRight = false;
                }
                else
                {
                    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x), sprite.localScale.y, sprite.localScale.z);
                    facingRight = true;
                }
            }
            else
            {
                // If the player is not within the activation range, stop chasing
                isShooting = false;
            }
        }
    }
    void EnemyShoot(bool isShooting)
    {
        if (isShooting)
        {
            // insiatiating the bullet at gunpoint poistion and rotation
            GameObject bull = Instantiate(bullet, gunPoint.transform.position, gunPoint.transform.rotation);


            //destroy bullet after 1 second
            Destroy(bull, destroyDelay);

            if (facingRight)
            {
                bull.GetComponent<Rigidbody2D>().AddForce(bull.GetComponent<Rigidbody2D>().transform.right * bulletVelocity);
            }
            else
            {
                bull.transform.localScale = new Vector3(bull.transform.localScale.x * -1, bull.transform.localScale.y, bull.transform.localScale.z);  // flip the bullet sprite
                bull.GetComponent<Rigidbody2D>().AddForce(-bull.GetComponent<Rigidbody2D>().transform.right * bulletVelocity);
            }
        }
    }


}