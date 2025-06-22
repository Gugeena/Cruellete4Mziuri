using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlinkoProjectileCode : MonoBehaviour
{
    public float bounceForce = 15f;
    public GameObject Particles;

    Rigidbody2D rb;

    public float torqMultiplier;
    public GameObject deathParticles;

    public AudioSource deathSound;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        float torque = Random.Range(-8.5f, 8.5f);
        if (torque >= 0) torque = Mathf.Clamp(torque, 7f, 8.5f);
        else torque = Mathf.Clamp(torque, -8.5f, -7f);
        rb.AddTorque(torque * (torqMultiplier + Random.Range(-0.4f, 0.4f)));
    }

    private void Update()
    {
        float clampedYvel = Mathf.Clamp(rb.velocity.y, -70, 70);
        rb.velocity = new Vector2(rb.velocity.x, clampedYvel);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("playerSlash"))
        {
            StartCoroutine(Death());
            PlinkoBossScript.hp--;
        }

        if (collision.gameObject.CompareTag("HeavyAttack"))
        {
            StartCoroutine(Death());
            PlinkoBossScript.hp -= 2 ;
        }

        if (collision.gameObject.CompareTag("LHAttack"))
        {
            StartCoroutine(Death());
            PlinkoBossScript.hp -= 2;
        }

        if (collision.gameObject.CompareTag("damageFloor"))
        {
            Instantiate(Particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }

    /*
    private IEnumerator Death()
    {
        Instantiate(Particles, transform.position, Quaternion.identity);
        deathSound.Play();
        Destroy(gameObject, 0.2f);
        yield return null;
    }
    */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(Particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("mp"))
        {
            Instantiate(Particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private IEnumerator Death()
    {
        Instantiate(Particles, transform.position, Quaternion.identity);
        deathSound.Play();
        yield return new WaitForSeconds(deathSound.clip.length);
        Destroy(gameObject);
    }
}
