using System.Collections.Generic;
using UnityEngine;

public class PlayeerController : MonoBehaviour
{
    [Header("Player Movement")]
//Buat Jump player
    public float moveSpeed = 10f;
    public Rigidbody2D rb;

    private float moveX;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxis("Horizontal") * moveSpeed;
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveX;
        rb.velocity = velocity;
    }
}
