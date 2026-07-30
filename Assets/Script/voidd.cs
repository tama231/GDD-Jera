using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voidd : MonoBehaviour
{
    public GameObject loseUi;

    void Start()
    {
        loseUi.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == "Player")
        {
            loseUi.SetActive(true);
            Time.timeScale = 0f;
            GameManager.GameIsOver = true;
        }
    }
}