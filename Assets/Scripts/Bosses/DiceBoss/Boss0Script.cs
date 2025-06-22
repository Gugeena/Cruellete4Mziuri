using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss0Script : MonoBehaviour
{
    public int hp = 25;
    Transform player;
    public float speed = 5f;
    public float stopDistance = 3f;
    public float attackCooldown = 0.6f;
    public float jumpHeight = 5f;
    public float jumpDuration = 0.5f;
    public float aoeDamageDuration = 0.5f;
    public float dashDistance = 8f;

    private bool isAttacking, isDashing, isJumping, canAttack;

    public GameObject DashIndicator1, DashIndicator2;
    public rouletteScript rouletteScript;
    public MovementScript movementScript;
    public GameObject Duo;

    public bool AllowedToDash = true;
    private bool isInContactWithPlayer = false;

    public Animator anim;
    Animator selfAnim;

    public GameObject deathParticles;

    private void Start()
    {
        isAttacking = isDashing = isJumping = false;
        canAttack = true;
        Debug.Log("Boss HP initialized to: " + hp);
        selfAnim = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
        StartCoroutine(AttackCycle());
    }

    private void Update()
    {
        print("hi");
        if (hp <= 0) death();

        if (!isAttacking && !isDashing && !isJumping && canAttack)
        {
            float distanceX = Mathf.Abs(transform.position.x - player.position.x);
            if (distanceX > stopDistance)
            {
                Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
            }
        }

        //if (rouletteScript != null)
        //{
        //    switch (rouletteScript.bossState)
        //    {
        //        case 0:
        //            FirstBuff();
        //            break;
        //        case 1:
        //            SecondBuff();
        //            break;
        //        case 2:
        //            ThirdBuff();
        //            break;
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning("Roulettescript component is missing");
        //}


    }

    private void FirstBuff()
    {
        Duo.SetActive(true);
        Debug.Log("BossState is 0");
    }

    private void SecondBuff()
    {
        Debug.Log("BossState is 1");
        movementScript.speed = 6;
    }
    private void ThirdBuff()
    {
        Debug.Log("BossState is 2");
        hp = 30;
        speed = 7;
        attackCooldown = 0.4f;
        Debug.Log(speed + " " + attackCooldown);
    }

    private void death()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
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
                if (attackChoice == 0 && !isInContactWithPlayer)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInContactWithPlayer = true;
            Debug.Log("Player is in contant with boss");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInContactWithPlayer = false;
            Debug.Log("Player is not in contant with boss");
        }
    }

    private IEnumerator DashAttack()
    {
        if (isInContactWithPlayer)
        {
            yield break;
        }

        isDashing = true;
        anim.SetBool("IsDashing", true);
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
               if (isInContactWithPlayer)
               {
                break;
               }
                transform.position = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / dashDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
                //anim.SetBool("IsDashing", true);
            }

            transform.position = targetPosition;
            DashIndicator2.SetActive(false);
            isDashing = false;
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("IsDashing", false);
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
        if (collision.gameObject.CompareTag("playerSlash"))
        {
            selfAnim.Play("Damage");
            hp--;
            Debug.Log("Boss HP decreased to: " + hp);
        }
    }
}