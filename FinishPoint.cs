using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }



    void OnTriggerStay2D(Collider2D other)
    {
       


        if (other.CompareTag("Player") && Input.GetKey(KeyCode.W))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}