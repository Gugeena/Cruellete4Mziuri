using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStuff : MonoBehaviour
{
    Rigidbody2D rb;
    public float torqMultiplier;
    public GameObject deathParticles;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        float torque = Random.Range(-8.5f, 8.5f);
        if(torque >= 0)torque = Mathf.Clamp(torque, 7f, 8.5f);
        else torque = Mathf.Clamp(torque, -8.5f, -7f);
        rb.AddTorque(torque * (torqMultiplier + Random.Range(-0.4f, 0.4f)));
        Destroy(gameObject, 5f);
    }
    private void Update()
    {
        float clampedYvel = Mathf.Clamp(rb.velocity.y, -70, 70);
        rb.velocity = new Vector2(rb.velocity.x, clampedYvel);
    }

    void onDeath()
    {
        Instantiate(deathParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("playerSlash") || collision.CompareTag("Player")) || collision.CompareTag("HeavyAttack") && deathParticles != null)
        {
            onDeath();
        }
    }
}
