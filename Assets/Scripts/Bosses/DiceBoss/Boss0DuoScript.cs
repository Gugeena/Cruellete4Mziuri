using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss0DuoScript : MonoBehaviour
{
    public int hp = 25;
    public Transform player;
    public float speed = 5f;
    public float stopDistance = 3f;
    public float attackCooldown = 0.6f;
    public float jumpHeight = 5f;
    public float jumpDuration = 0.5f;
    public float aoeDamageDuration = 0.5f;
    public float dashDistance = 8f;

    private bool isAttacking = false;
    private bool isDashing = false;
    private bool isJumping = false;
    private bool canAttack = true;
    public GameObject DashIndicator1;
    public GameObject DashIndicator2;
    public Roulettescript rouletteScript;
    public MovementScript movementScript;

    private void Start()
    {
        StartCoroutine(AttackCycle());
        Debug.Log("Boss HP initialized to: " + hp);
    }

    private void Update()
    {
        if (!isAttacking && !isDashing && !isJumping && canAttack)
        {
            float distanceX = Mathf.Abs(transform.position.x - player.position.x);
            if (distanceX > stopDistance)
            {
                Vector2 direction = new Vector2((player.position.x - transform.position.x), 0).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
            }
        }

        if (hp <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.Log("Boss has died.");
        //particle da sound
        Destroy(gameObject);
    }
    private IEnumerator AttackCycle()
    {
        while (true)
        {
            if (!isAttacking && Vector2.Distance(transform.position, player.position) <= stopDistance && canAttack)
            {
                isAttacking = true;

                int attackChoice = Random.Range(0, 2);
                if (attackChoice == 0)
                {
                    yield return StartCoroutine(DashAttack());
                }
                else
                {
                    yield return StartCoroutine(JumpAttack());
                }

                isAttacking = false;
                canAttack = false;

                yield return new WaitForSeconds(attackCooldown);
                canAttack = true;
            }

            yield return null;
        }
    }

    private IEnumerator DashAttack()
    {
        isDashing = true;
        isAttacking = true;
        DashIndicator1.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        DashIndicator1.SetActive(false);
        DashIndicator2.SetActive(true);

        Vector2 playerPosLocked = new Vector2(player.transform.position.x, transform.position.y);
        Vector2 dashDirection = (playerPosLocked - (Vector2)transform.position).normalized;
        Vector2 targetPosition = (Vector2)transform.position + dashDirection * dashDistance;
        float dashDuration = 0.2f;
        float elapsedTime = 0f;
        Vector2 startingPosition = transform.position;

        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        DashIndicator2.SetActive(false);
        isDashing = false;
        isAttacking = false;
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator JumpAttack()
    {
        isJumping = true;

        Vector2 initialPosition = transform.position;
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        float jumpProgress = 0;

        while (jumpProgress < 1)
        {
            jumpProgress += Time.deltaTime / jumpDuration;
            float jumpY = Mathf.Lerp(initialPosition.y, targetPosition.y + jumpHeight, Mathf.Sin(jumpProgress * Mathf.PI));
            transform.position = new Vector2(Mathf.Lerp(initialPosition.x, targetPosition.x, jumpProgress), jumpY);

            yield return null;
        }

        yield return new WaitForSeconds(aoeDamageDuration);
        isJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            hp--;
            Debug.Log("Boss HP decreased to: " + hp);
        }
    }
}