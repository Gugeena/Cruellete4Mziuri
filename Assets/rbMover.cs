using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rbMover : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    private float xSpeed, ySpeed;
    public static float MULTIPLIER = 1;
    private float spawnTime;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(xSpeed, ySpeed) * MULTIPLIER;
    }
}
