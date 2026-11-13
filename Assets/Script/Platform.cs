using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;

    [Header("Jump Sound")]
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioClip jumpSound;

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Max(1f, value);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        if (collision.relativeVelocity.y > 1f)
            return;

        Collider2D platformCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = collision.collider;
        if (platformCollider != null && playerCollider != null)
        {
            float playerBottom = playerCollider.bounds.min.y;
            float platformTop = platformCollider.bounds.max.y;

            if (playerBottom > platformTop - 0.1f)
                return;
        }

        Vector2 velocity = rb.velocity;
        if (velocity.y <= 0f)
        {
            velocity.y = jumpForce;
            rb.velocity = velocity;
            PlayJumpSound();
        }
    }

    private void PlayJumpSound()
    {
        if (jumpAudioSource != null && jumpSound != null)
            jumpAudioSource.PlayOneShot(jumpSound);
    }
}



