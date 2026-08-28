using System.Collections;
using UnityEngine;

public class PlayeerController : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 10f;
    public Rigidbody2D rb;

    private float moveX;
    private float speedMultiplier = 1f;
    private float jumpMultiplier = 1f;
    private float gameSpeedMultiplier = 1f;
    private Coroutine speedEffectCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxis("Horizontal") * moveSpeed * speedMultiplier;
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveX;
        rb.velocity = velocity;
    }

    public void ApplySpeedEffect(float multiplier, float duration)
    {
        if (speedEffectCoroutine != null)
            StopCoroutine(speedEffectCoroutine);

        speedEffectCoroutine = StartCoroutine(SpeedEffectRoutine(
            Mathf.Max(0.1f, multiplier), Mathf.Max(0f, duration)));
    }

    public float GetJumpMultiplier()
    {
        return jumpMultiplier;
    }

    public void ApplyJumpBoost(float multiplier)
    {
        jumpMultiplier *= Mathf.Max(0.1f, multiplier);
    }

    public void ApplyGameSpeed(float multiplier)
    {
        gameSpeedMultiplier *= Mathf.Max(0.1f, multiplier);
        Time.timeScale = gameSpeedMultiplier;
    }

    public void ClearDebuffs()
    {
        gameSpeedMultiplier = 1f;
        jumpMultiplier = 1f;
        Time.timeScale = 1f;
    }

    private IEnumerator SpeedEffectRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        speedEffectCoroutine = null;
    }
}
