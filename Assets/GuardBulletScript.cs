using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBulletScript : MonoBehaviour
{
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(150 * transform.right);
        Destroy(gameObject, 7);

    }
}
