using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouncingOrange : MonoBehaviour
{
    Rigidbody2D rb;
    public float regularGravity, fallGravity, initialForce, maxVel;
    bool explodeOnCollision;
    public GameObject particles;
    public float torqMultiplier;

    private void Awake()
    {
        explodeOnCollision = false;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * initialForce * -1);
        StartCoroutine(life());
        float torque = Random.Range(-8.5f, 8.5f);
        if (torque >= 0) torque = Mathf.Clamp(torque, 7f, 8.5f);
        else torque = Mathf.Clamp(torque, -8.5f, -7f);
        rb.AddTorque(torque * (torqMultiplier + Random.Range(-0.4f, 0.4f)));
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxVel, maxVel), Mathf.Clamp(rb.velocity.y, -maxVel, maxVel));

        if(rb.velocity.y < 0) rb.gravityScale = fallGravity;
        else rb.gravityScale = regularGravity;
    }

    private IEnumerator life()
    {
        yield return new WaitForSeconds(5.5f);
        explodeOnCollision = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (explodeOnCollision && collision.gameObject.layer == 3){
            Instantiate(particles, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
