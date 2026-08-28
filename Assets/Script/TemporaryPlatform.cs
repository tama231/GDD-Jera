using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    public float jumpForce = 10f;

    private bool hasBeenUsed;
    private int durabilityBonus;

    public void SetDurabilityBonus(int bonus)
    {
        durabilityBonus = Mathf.Max(0, bonus);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenUsed)
            return;

        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (durabilityBonus > 0)
                    durabilityBonus--;
                else
                    hasBeenUsed = true;
                Vector2 velocity = rb.velocity;
                PlayeerController player = collision.gameObject.GetComponent<PlayeerController>();
                float jumpMultiplier = player != null ? player.GetJumpMultiplier() : 1f;
                velocity.y = jumpForce * jumpMultiplier;
                rb.velocity = velocity;
                if (hasBeenUsed)
                    Destroy(gameObject);
            }
        }
    }
}
