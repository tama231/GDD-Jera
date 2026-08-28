using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    public float jumpForce = 10f;

    private bool hasBeenUsed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenUsed)
            return;

        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                hasBeenUsed = true;
                Vector2 velocity = rb.velocity;
                velocity.y = jumpForce;
                rb.velocity = velocity;
                Destroy(gameObject);
            }
        }
    }
}
