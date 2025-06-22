using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonteExplosiveCardScript : MonoBehaviour
{
    Rigidbody2D rb;
    public float torqueMultiplier;
    public GameObject deathParticles;
    public GameObject Explosion;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        float torque = Random.Range(-8.5f, 8.5f);
        if (torque >= 0) torque = Mathf.Clamp(torque, 7f, 8.5f);
        else torque = Mathf.Clamp(torque, -8.5f, -7f);
        rb.AddTorque(torque * (torqueMultiplier + Random.Range(-0.4f, 0.4f)));
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(Death());
        }

        if(collision.gameObject.name == "Ground")
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        Instantiate(deathParticles, transform.position, transform.rotation);
        Instantiate(Explosion, transform.position, transform.rotation);
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
