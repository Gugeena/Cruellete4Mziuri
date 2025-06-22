using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class GuardBossAI : MonoBehaviour
{
    [Header("HandAttack1")]

    [SerializeField]
    private GameObject handAttack1;

    [SerializeField]
    private Transform leftPosition, rightPosition;

    [SerializeField]
    private float attackForce, hand1IdleTime;


    [Header("CircleShootAttack")]
    [SerializeField]
    private GameObject bat, shooter;
    [SerializeField]
    private Animator popOut;
    [SerializeField]
    private float batIdleTime, attackTime;

    [Header("TopHandAttack2")]
    [SerializeField]
    private GameObject[] hands;
    [SerializeField]
    private Animator handsAnim;

    [Header("BatsAttack")]
    [SerializeField]
    private ObjectSpawner topBatSpawner;
    [SerializeField]
    private ObjectSpawner botBatSpawner;
    [SerializeField]
    private GameObject topBatParent, botBatParent;

    [Header("Misc")]
    [SerializeField]
    private GameObject shakeCam;
    private GameObject player;
    private bool spinQueued, isInvulnerable;
    [SerializeField]
    private rouletteScript rs;
    [SerializeField]
    private Sprite[] toastSprites;
    [SerializeField]
    private float idleTime;
    private camShakerScript cShakeS;
    private int lastAttack, nextAttack, repeatCount, attackCounter;

    private void Start()
    {
        player = GameObject.Find("Player");
        cShakeS = GetComponent<camShakerScript>();
        //chooseAttack();
        StartCoroutine(batsAttack());
    }

    void chooseAttack()
    {
        
        if (player == null)
        {
            return;
        }

        if (spinQueued)
        {
            rs.StartCoroutine(rs.spinCRT(toastSprites, 4));
            spinQueued = false;
            isInvulnerable = true;
        }
        else
        {
         
            lastAttack = nextAttack;
            nextAttack = Random.Range(0, 2);
            if (nextAttack == lastAttack)
            {
                repeatCount++;
                print("repeat!");
            }
            else repeatCount = 0;
            if (repeatCount >= 3) { 
                chooseAttack();
                return;
            }
            if (attackCounter % 7 == 0 && attackCounter != 0) StartCoroutine(circleShootAttack());
            else if (nextAttack == 0) StartCoroutine(handFallAttack(Random.Range(0, 2) == 0));
            else if (nextAttack == 1) StartCoroutine(topHandSmashAttack());

            attackCounter++;
        }
    }

    private IEnumerator handFallAttack(bool rightToLeft)
    {
        ParticleSystem grindParticles;
        Rigidbody2D rb;
        ShakeSelfScript shakescript;
        GameObject hand;
        if (rightToLeft)
        {
            hand = Instantiate(handAttack1, rightPosition.position, Quaternion.identity);
            rb = hand.GetComponent<Rigidbody2D>();
            grindParticles = hand.transform.GetChild(1).GetComponent<ParticleSystem>();
            shakescript = hand.transform.GetChild(0).GetComponent<ShakeSelfScript>();
            yield return new WaitForSeconds(hand1IdleTime - 1f);
            shakescript.Begin();
            yield return new WaitForSeconds(1f);
            rb.AddForce(attackForce * -100 * transform.right);

        }
        else
        {
            hand = Instantiate(handAttack1, leftPosition.position, Quaternion.identity);
            rb = hand.GetComponent<Rigidbody2D>();
            grindParticles = hand.transform.GetChild(1).GetComponent<ParticleSystem>();
            shakescript = hand.transform.GetChild(0).GetComponent<ShakeSelfScript>();
            yield return new WaitForSeconds(hand1IdleTime - 1f);
            shakescript.Begin();
            yield return new WaitForSeconds(1f);
            rb.AddForce(attackForce * 100 * transform.right);
        }
        shakeCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
        
        grindParticles.Play();
        yield return new WaitForSeconds(1f);
        grindParticles.Stop();
        shakeCam.GetComponent<CinemachineVirtualCamera>().enabled = false;
        yield return new WaitForSeconds(1f);
        shakescript.stopShake();
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x, 65);
        yield return new WaitForSeconds(1f);
        Destroy(hand);
        chooseAttack();
    }

    private IEnumerator circleShootAttack()
    {
        shooter.SetActive(true);
        shooter.transform.GetComponentInParent<Animator>().Play("ShooterComeDown");
        yield return new WaitForSeconds(2.5f);
        shooter.GetComponent<ShooterScript>().startShooting();
        yield return new WaitForSeconds(2f);
        float attackTime = Time.time + this.attackTime + Random.Range(0.1f, 2.5f);
        while (Time.time < attackTime) {
            Vector2 pos = new Vector2(player.transform.position.x, popOut.transform.position.y);
            GameObject batObj = Instantiate(bat, pos, Quaternion.identity, popOut.gameObject.transform);
            Destroy(batObj, 3.4f);
            popOut.Play("batPopOut");  
            yield return new WaitForSeconds(batIdleTime);
        }
        shooter.GetComponent<ShooterScript>().stopShooting();
        shooter.GetComponentInParent<Animator>().Play("ShooterGoUp");
        chooseAttack();
    }

    private IEnumerator topHandSmashAttack()
    {
        hands[Random.Range(0, hands.Length)].SetActive(false);
        handsAnim.Play("topHandSmash");
        yield return new WaitForSeconds(2.45f);
        StartCoroutine(cShakeS.shake());
        yield return new WaitForSeconds(3f);
        foreach (GameObject h in hands)
        {
            h.SetActive(true);
        } 
        chooseAttack();
    }

    private IEnumerator batsAttack() {
        topBatSpawner.startSpawning();
        botBatSpawner.startSpawning();
        yield return new WaitForSeconds(7);
        topBatParent.GetComponent<Animator>().Play("topBatsMove");

    }

}
