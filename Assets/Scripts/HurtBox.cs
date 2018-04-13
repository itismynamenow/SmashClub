using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class HurtBox : MonoBehaviour 
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Health>().takeDamage(10);
            //SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
        }
    }
}

