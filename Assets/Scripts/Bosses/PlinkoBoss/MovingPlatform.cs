using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float directionX = 1;
    public float directionY = 1;
    public static float speedX, speedY;
    Rigidbody2D rb;
    MovementScript ms;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(speedX * directionX, speedY * directionY);
    }

}
