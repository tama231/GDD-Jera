using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.velocity;
                PlayeerController player = collision.gameObject.GetComponent<PlayeerController>();
                float jumpMultiplier = player != null ? player.GetJumpMultiplier() : 1f;
                velocity.y = jumpForce * jumpMultiplier;
                rb.velocity = velocity;
            }
        }
    }
}



