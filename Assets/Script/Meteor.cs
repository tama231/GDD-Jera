using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("Meteor Settings")]
    public float fallSpeed = 2f;
    public Rigidbody2D rb;
    [Header("Meteor Audio")]
    public AudioSource audioSource;
    public AudioClip collideSound;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.down * 10f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.Die();
            GameManager.GameIsOver = true;

        audioSource.PlayOneShot(collideSound);
        Destroy(gameObject);

        if (other.CompareTag("void"))
        {
            Destroy(gameObject);
        }
    }
}

