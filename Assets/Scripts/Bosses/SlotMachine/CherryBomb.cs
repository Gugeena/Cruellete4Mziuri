using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryBomb : MonoBehaviour
{
    Rigidbody2D rb;
    bool active = true;
    GameObject player;

    public GameObject particles;
    public float speed, knockback, maxVel;

    int hp = 10;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.rotation = angle;


        if(hp <= 0)
        {
            Instantiate(particles, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            rb.AddForce(transform.right * speed);
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxVel, maxVel), Mathf.Clamp(rb.velocity.y, -maxVel, maxVel));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("playerSlash"))
        {
            rb.AddForce(-transform.right * knockback * 100);
            anim.Play("orangeDamage");
            hp--;
        }

        if (collision.gameObject.CompareTag("HeavyAttack"))
        {
            rb.AddForce(-transform.right * knockback * 270);
            anim.Play("orangeDamage");
            hp -= 3;
        }
    }
}
