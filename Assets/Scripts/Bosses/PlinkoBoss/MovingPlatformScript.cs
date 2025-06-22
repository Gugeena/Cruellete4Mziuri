using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingPlatformScript : MonoBehaviour
{
    public float directionX = 1;
    public float directionY = 1;
    public static float speedX, speedY;
    Rigidbody2D rb;
    MovementScript ms;

    bool isFalling;

    public AudioSource fallSound;

    public static bool falling;

    public Scene activeScene;

    void Awake()
    {
        isFalling = false;
        rb = GetComponent<Rigidbody2D>();
        activeScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isFalling)rb.velocity = new Vector2(speedX * directionX, speedY * directionY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("mpremover"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            collision.gameObject.transform.parent = this.transform;

            if (falling)
            {
                StartCoroutine(cooldownfall());
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }

    private IEnumerator cooldownfall()
    {
        if (activeScene.name == "BuffedPlinkoBoss")
        {
            yield return new WaitForSeconds(1f);
            rb.isKinematic = false;
            rb.AddForce(100 * -transform.up);
            rb.gravityScale = 1f;
            fallSound.Play();
            isFalling = true;
            Destroy(gameObject, 3f);
        }
        else
        {
            yield return new WaitForSeconds(1.8f);
            rb.isKinematic = false;
            rb.AddForce(100 * -transform.up);
            rb.gravityScale = 1f;
            fallSound.Play();
            isFalling=true;
            Destroy(gameObject, 3f);
        }
    }
}