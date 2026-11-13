using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;

    private bool hasBeenUsed;
    private int durabilityBonus;

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Max(1f, value);
    }

    public void SetDurabilityBonus(int bonus)
    {
        durabilityBonus = Mathf.Max(0, bonus);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenUsed)
            return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        if (collision.relativeVelocity.y > 1f)
            return;

        if (durabilityBonus > 0)
            durabilityBonus--;
        else
            hasBeenUsed = true;

        Vector2 velocity = rb.velocity;
        if (velocity.y <= 0f)
        {
            velocity.y = jumpForce;
            rb.velocity = velocity;
        }

        if (hasBeenUsed)
            Destroy(gameObject);
    }
}
