using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tagKiller : MonoBehaviour
{
    public string tagToKill;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagToKill))
        {
            Destroy(collision.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tagToKill))
        {
            Destroy(collision.gameObject);
        }
    }
}
