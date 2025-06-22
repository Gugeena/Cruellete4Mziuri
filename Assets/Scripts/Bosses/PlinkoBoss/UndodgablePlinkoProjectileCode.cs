using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndodgablePlinkoProjectileCode : MonoBehaviour
{
    public GameObject Particles;
    Rigidbody2D rb;
    public float torqMultiplier;
    public GameObject deathParticles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        float torque = Random.Range(-8.5f, 8.5f);
        if (torque >= 0) torque = Mathf.Clamp(torque, 7f, 8.5f);
        else torque = Mathf.Clamp(torque, -8.5f, -7f);
        rb.AddTorque(torque * (torqMultiplier + Random.Range(-0.4f, 0.4f)));
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        float clampedYvel = Mathf.Clamp(rb.velocity.y, -70, 70);
        rb.velocity = new Vector2(rb.velocity.x, clampedYvel);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("damageFloor"))
        {
            Instantiate(Particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }

    private IEnumerator Death()
    {
        Instantiate(Particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("mp"))
        {
            Instantiate(Particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Instantiate(Particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
