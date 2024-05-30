using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Start is called before the first frame update

    ShopManager shop;

    private void Start()
    {
        shop = FindObjectOfType<ShopManager>();
    }

    public AudioSource audioSource;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" )
        {
            audioSource.Play();
            Destroy(gameObject);
            shop.AddCoins();
        }
    }
}
