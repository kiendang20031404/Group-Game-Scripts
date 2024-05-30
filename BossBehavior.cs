using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{

    private Color flashColor = Color.red; // Color of the flash
    private Color originalColor; // Original color of the enemy

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    private Transform player; // Reference to the player's transform

    private Animator anim;  //animation for enemy movement


    private bool facingRight;
    private bool isHit = false;
    private int currentHealth;
    private float flashDuration = 0.1f;
    public float followSpeed = 5f; // Speed at which the enemy follows the player
    public float activationDistance = 5f; // Distance at which following behavior is activated


    private float timeBetweenShots = 0.2f;  // Time between each shot in a burst
    private float timeBetweenBursts = 2f;   // Time between each burst

    private float burstTimer;  // Timer to keep track of burst intervals
    private int bulletsFired;   // Counter for bullets fired in the current burst


    [Header("Enemy Properties")]

    public int startingHealth;
    public HealthBar healthBar;
    public GameObject bullet;  // bullet sprite

    public GameObject bullet2;


    public GameObject gunPoint;  // the position where the bullet fires

    private Rigidbody2D rigid;  // rigidbody for recoil tuning
    private Transform sprite;

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

        facingRight = true;

        burstTimer = 0f;
        bulletsFired = 0;
    }

    void Update()
    {
        anim.SetFloat("Speed", Mathf.Abs(rigid.velocity.x));
        anim.SetBool("touchingGround", true);
        if (isHit)
        {
            Invoke("ResetHitFlag", 0.5f);
        }



        EnemyAi();

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

    void EnemyAi()
    {

        // If player is still alive
        if (player != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the distance is within the activation range
            if (distanceToPlayer <= activationDistance)
            {

                if (burstTimer <= 0f)
                {
                    int rand = UnityEngine.Random.Range(0, 2); // Generates either 0 or 1
                    StartCoroutine(EnemyShoot(rand));
                    burstTimer = timeBetweenBursts;
                }
                else
                {
                    // Countdown the burst timer
                    burstTimer -= Time.deltaTime;
                }

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


    IEnumerator EnemyShoot(int mode)
    {

        bulletsFired = 0;

        if (mode == 0)
        {
            while (bulletsFired < 3)
            {
                if (facingRight)
                {

                    // insiatiating the bullet at gunpoint poistion and rotation
                    GameObject bull = Instantiate(bullet2, gunPoint.transform.position, gunPoint.transform.rotation);

                    Rigidbody2D BulletRigid = bull.GetComponent<Rigidbody2D>();

                    Destroy(bull, 1.5f); //destroy bullet after 1.5s

                    BulletRigid.AddForce(BulletRigid.transform.right * 600); // fire the bullet with 500 force
                }
                else
                {
                    GameObject bull = Instantiate(bullet2, gunPoint.transform.position, gunPoint.transform.rotation);

                    Rigidbody2D BulletRigid = bull.GetComponent<Rigidbody2D>();

                    bull.transform.localScale = new Vector3(bull.transform.localScale.x * -1, bull.transform.localScale.y, 0);  // flip the bullet sprite

                    Destroy(bull, 1.5f); //destroy bullet after 1.5s

                    BulletRigid.AddForce(-BulletRigid.transform.right * 600); // fire the bullet with force 500
                }

                bulletsFired++;
                yield return new WaitForSeconds(timeBetweenShots);
            }
        }
        else
        {
            while (bulletsFired < 1)
            {
                if (facingRight)
                {

                    // insiatiating the bullet at gunpoint poistion and rotation
                    GameObject bull = Instantiate(bullet, gunPoint.transform.position, gunPoint.transform.rotation);

                    Rigidbody2D BulletRigid = bull.GetComponent<Rigidbody2D>();

                    Destroy(bull, 2f); //destroy bullet after 2s

                    BulletRigid.AddForce(BulletRigid.transform.right * 1000); // fire the bullet with force of 1000
                }
                else
                {

                    // insiatiating the bullet at gunpoint poistion and rotation
                    GameObject bull = Instantiate(bullet, gunPoint.transform.position, gunPoint.transform.rotation);

                    bull.transform.localScale = new Vector3(bull.transform.localScale.x * -1, bull.transform.localScale.y, 0);  // flip the bullet sprite

                    Rigidbody2D BulletRigid = bull.GetComponent<Rigidbody2D>();

                    Destroy(bull, 2f);

                    BulletRigid.AddForce(-BulletRigid.transform.right * 1000);
                }

                bulletsFired++;
                yield return new WaitForSeconds(timeBetweenShots);
            }

        }
    }

}

