using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class miniDiceBossScript : MonoBehaviour
{
    Rigidbody2D rb;
    //public GameObject shakeCam;
    public int hp = 8;

    public float dashForce;

    bool inRange;

    int direction;
    public float speed;
    public float distance;
    public GameObject grindParticles;

    public float jumpHeight, jumpSpeed;
    Vector2 jumpTargetPos;
    public bool isJumping;
    BoxCollider2D bc;

    GameObject player;

    Animator spriteAnim;

    bool isAttacking;

    int playerDir;
    bool dead;

    MovementScript movementScript;

    public AudioSource[] sounds;
    AudioManager audioManager;

    public GameObject deathParticles;

    public Scene currentScene;

    SlotBossAI slotBossAI;
    public LayerMask bossLayer;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "BuffedSlotMachine") hp = 8;
        else hp = 10;
        bc = GetComponent<BoxCollider2D>();
        isAttacking = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        slotBossAI = GameObject.Find("SlotBoss").GetComponent<SlotBossAI>();
        movementScript = player.GetComponent<MovementScript>();
        spriteAnim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        StartCoroutine(GateKeeper());
    }

    // Update is called once per frame
    void Update()
    {
        handleHP();

        if (!dead)
        {
            if (!isAttacking) handleMovement();
        }

        if (slotBossAI.hp <= 25) StartCoroutine(deathCRT());
    }

    void handleHP()
    {
        if (hp <= 0 && !dead) StartCoroutine(deathCRT());
    }

    void handleMovement()
    {
        inRange = Vector2.Distance(transform.position, player.transform.position) <= distance;
        if (inRange)
        {
            chooseAttack();
            if (grindParticles.activeSelf) grindParticles.SetActive(false);
        }
        if (transform.position.x - player.transform.position.x >= 0) playerDir = 1;
        else playerDir = -1;

        if (playerDir < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            grindParticles.transform.localScale = new Vector3(grindParticles.transform.localScale.x * -1, grindParticles.transform.localScale.y, grindParticles.transform.localScale.z);
        }
        else if (playerDir > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            grindParticles.transform.localScale = new Vector3(Mathf.Abs(grindParticles.transform.localScale.x), grindParticles.transform.localScale.y, grindParticles.transform.localScale.z);
        }


    }

    private void FixedUpdate()
    {
        if (!inRange && !isAttacking)
        {
            if (!grindParticles.activeSelf) grindParticles.SetActive(true);
            //if (rb.isKinematic) rb.isKinematic = false;
            Vector2 velocity = new Vector2(-speed * Mathf.Clamp(transform.position.x - player.transform.position.x, -1, 1), rb.velocity.y);
            rb.velocity = velocity;
        }

    }



    void chooseAttack()
    {
        int nextAttack = Random.Range(0, 11);
        if (nextAttack >= 0 && nextAttack <= 3) StartCoroutine(jumpAttack());
        else StartCoroutine(dashAttack());
    }



    private IEnumerator jumpAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        jumpTargetPos = new Vector2(player.transform.position.x, jumpHeight);
        isJumping = true;
        audioManager.playAudio(sounds[2]);
        bc.enabled = false;
        rb.gravityScale = 8f;
        yield return new WaitForSeconds(0.5f);
        rb.MovePosition(jumpTargetPos);
        spriteAnim.Play("bossSpin");
        yield return new WaitForSeconds(0.35f);
        bc.enabled = true;
        isJumping = false;
        yield return new WaitForSeconds(0.35f);
        rb.gravityScale = 1;
        isAttacking = false;
    }

    private IEnumerator deathCRT()
    {
        dead = true;
        audioManager.playAudio(sounds[3]);
        Instantiate(deathParticles, transform.position, transform.rotation);
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator dashAttack()
    {
        isAttacking = true;
        spriteAnim.Play("bossDash");
        yield return new WaitForSeconds(0.7f);
        rb.gravityScale = 9;
        audioManager.playAudio(sounds[1]);
        rb.AddForce(dashForce * 1000 * transform.right * -playerDir);
        yield return new WaitForSeconds(1.5f);
        rb.gravityScale = 1;
        isAttacking = false;

    }
    //private IEnumerator screenShake()
    //{
    //    audioManager.playAudio(sounds[Random.Range(2, 4)]);
    //    shakeCam.SetActive(true);
    //    yield return new WaitForSeconds(0.15f);
    //    shakeCam.SetActive(false);
    //}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && !collision.gameObject.CompareTag("Player"))
        {
            //StartCoroutine(screenShake());
            transform.localEulerAngles = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("playerSlash"))
        {
            audioManager.playAudio(sounds[0]);
            hp -= 1;
            spriteAnim.Play("damage");
        }

        if (collision.CompareTag("HeavyAttack"))
        {
            audioManager.playAudio(sounds[0]);
            hp -= 2;
            spriteAnim.Play("damage");
        }
    }

    public IEnumerator GateKeeper()
    {
        yield return new WaitForSeconds(12.5f);
        print("hi");
        rb.includeLayers = bossLayer;
        bc.includeLayers = bossLayer;
    }
}
