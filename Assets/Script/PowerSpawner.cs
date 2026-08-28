using UnityEngine;

public class PowerSpawner : MonoBehaviour
{
    public GameObject powerPrefab;
    public Transform player;
    public float spawnInterval = 7f;
    public float spawnAhead = 15f;
    public float minX = -6f;
    public float maxX = 6f;
    [Range(0f, 1f)] public float debuffChance = 0.3f;
    public float buffMultiplier = 1.8f;
    public float debuffMultiplier = 0.45f;
    public float effectDuration = 5f;

    private float nextSpawnY;

    void Start()
    {
        if (player == null)
        {
            PlayeerController controller = FindObjectOfType<PlayeerController>();
            if (controller != null)
                player = controller.transform;
        }

        nextSpawnY = transform.position.y;
    }

    void Update()
    {
        if (player == null)
            return;

        while (nextSpawnY < player.position.y + spawnAhead)
        {
            nextSpawnY += spawnInterval;
            SpawnPower(new Vector3(Random.Range(minX, maxX), nextSpawnY, 0f));
        }
    }

    private void SpawnPower(Vector3 position)
    {
        GameObject power = powerPrefab != null
            ? Instantiate(powerPrefab, position, Quaternion.identity)
            : CreateFallbackPower(position);

        PowerPickup pickup = power.GetComponent<PowerPickup>();
        if (pickup == null)
            pickup = power.AddComponent<PowerPickup>();

        PowerType type = Random.value < debuffChance ? PowerType.Debuff : PowerType.Buff;
        pickup.Setup(type, type == PowerType.Buff ? buffMultiplier : debuffMultiplier, effectDuration);
    }

    private GameObject CreateFallbackPower(Vector3 position)
    {
        GameObject power = new GameObject("Power");
        power.transform.position = position;
        power.transform.localScale = Vector3.one * 0.7f;
        CircleCollider2D collider = power.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        SpriteRenderer renderer = power.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), Vector2.one * 0.5f);
        return power;
    }
}

public enum PowerType { Buff, Debuff }

public class PowerPickup : MonoBehaviour
{
    private PowerType type;
    private float multiplier;
    private float duration;

    public void Setup(PowerType newType, float newMultiplier, float newDuration)
    {
        type = newType;
        multiplier = newMultiplier;
        duration = newDuration;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = type == PowerType.Buff ? Color.cyan : Color.red;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayeerController player = other.GetComponent<PlayeerController>();
        if (player == null)
            return;

        player.ApplySpeedEffect(multiplier, duration);
        Destroy(gameObject);
    }
}
