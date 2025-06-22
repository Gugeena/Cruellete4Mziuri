using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class dBossScript : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject shakeCam;
    bool spinQueued;
    public rouletteScript rs;
    public Sprite[] toastSprites;
    public bool isInvulnerable;
    public static int hp = 45;

    public float dashForce;

    bool inRange;

    int direction;
    public float speed;
    public float distance;
    public GameObject grindParticles;

    public float jumpHeight;
    Vector2 jumpTargetPos;
    public bool isJumping;
    public float fallGravity;
    BoxCollider2D bc;

    GameObject player;

    Animator spriteAnim;

    bool isAttacking;

    int playerDir;
    float hpCurVel = 0f;
    public UnityEngine.UI.Slider hpSlider;
    bool dead;

    public int currBuff;
    public bool isSpinningR;

    MovementScript movementScript;

    public bool isClone;
    public static bool cloneSpawned;
    public GameObject clone;

    public AudioSource[] sounds;
    public AudioManager audioManager;

    GameObject duo;

    bool musicPlaying;
    private double nextEventTime;

    public GameObject deathParticles;
    public Animator fadeOutAnim;

    AudioLowPassFilter lowPass;

    String Currentscene;

    private bool isDoubleDownActive = false;

    void Start()
    {
        Currentscene = SceneManager.GetActiveScene().name;
        cloneSpawned = false;

        if (Currentscene == "BuffedDiceBOSS")
        {
            if (!isClone) hp = 60;
            print("Initial HP (BuffedDiceBOSS): " + hp);
        }
        else
        {
            if (!isClone) hp = 45;
            print("Initial HP (Default): " + hp);
        }

        bc = GetComponent<BoxCollider2D>();
        isAttacking = false;
        isInvulnerable = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        movementScript = player.GetComponent<MovementScript>();
        spriteAnim = transform.GetChild(0).gameObject.GetComponent<Animator>();

        if (isClone)
        {
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            shakeCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>(true).gameObject;
        }

        if (!isClone)
        {
            sounds[1].clip.LoadAudioData();
            audioManager.playAudio(sounds[0]);
            nextEventTime = AudioSettings.dspTime + sounds[0].clip.length;
        }

        lowPass = GetComponent<AudioLowPassFilter>();

        if (Currentscene == "BuffedDiceBOSS")
        {
            spinQueued = true;
        }
    }

    void Update()
    {
        if (isInvulnerable)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
            
        print(hp);
        handleHP();

        if (!dead)
        {
            if (!isAttacking) handleMovement();
            if (isClone && !cloneSpawned)
            {
                cloneSpawned = true;
                StartCoroutine(cloneSpawn());
            }
            if (!isClone) handleMusic();
        }

        if (!isClone)
        {
            if (PauseMenu.gamePaused) lowPass.enabled = true;
            else lowPass.enabled = false;
        }

        Debug.Log(gameObject.name + " isAttacking: " + isAttacking);
    }

    void handleMusic()
    {
        double time = AudioSettings.dspTime;

        if (!musicPlaying && time + 1.0f > nextEventTime)
        {
            musicPlaying = true;
            audioManager.playScheduled(sounds[1], nextEventTime);
        }
    }

    void handleHP()
    {
        if (!isClone)
        {
            hp = Mathf.Clamp(hp, 0, 60);
            float currHP = Mathf.SmoothDamp(hpSlider.value, hp, ref hpCurVel, 0.2f);
            hpSlider.value = currHP;

            print("HP after clamping: " + hp);

            if (Currentscene == "BuffedDiceBOSS")
            {
                if ((hp <= 40 && rs.spinCount <= 1) || (hp <= 20 && rs.spinCount <= 2) || (hp <= 10 && rs.spinCount <= 3) && !isClone)
                {
                    spinQueued = true;
                    print("Spin queued at HP: " + hp);
                }
            }
            else
            {
                if ((hp <= 31 && rs.spinCount <= 0) || (hp <= 14 && rs.spinCount <= 1) && !isClone)
                {
                    spinQueued = true;
                    print("Spin queued at HP: " + hp);
                }
            }
        }

        if (hp <= 0 && !dead) StartCoroutine(deathCRT());
    }

    void handleMovement()
    {
        if (!dead)
        {
            inRange = Vector2.Distance(transform.position, player.transform.position) <= distance;
            if (inRange && !isInvulnerable)
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
    }

    private void FixedUpdate()
    {
        if (!inRange && !isAttacking)
        {
            if (!grindParticles.activeSelf) grindParticles.SetActive(true);
            Vector2 velocity = new Vector2(-speed * Mathf.Clamp(transform.position.x - player.transform.position.x, -1, 1), rb.velocity.y);
            rb.velocity = velocity;
        }
    }

    void chooseAttack()
    {
        if (player == null)
        {
            Debug.LogError(gameObject.name + " can't find the player in BuffedDiceBOSS scene!");
            return;
        }

        if (spinQueued && !isClone)
        {
            rs.StartCoroutine(rs.spinCRT(toastSprites, 4));
            spinQueued = false;
            isInvulnerable = true;
        }
        else
        {
            int nextAttack = UnityEngine.Random.Range(0, 11);
            if (duo == null)
            {
                if (nextAttack >= 0 && nextAttack <= 4) StartCoroutine(jumpAttack());
                else StartCoroutine(dashAttack());
            }
            else
            {
                if (nextAttack >= 0 && nextAttack <= 4 && !duo.GetComponent<dBossScript>().isJumping) StartCoroutine(jumpAttack());
                else StartCoroutine(dashAttack());
            }
        }
    }

    public void handleBuffs()
    {
        if (Currentscene == "BuffedDiceBOSS")
        {
            if (currBuff == 0)
            {
                chooseAttack();
                return;
            }
            else if (currBuff == 1)
            {
                dashForce = 390;
                fallGravity = 12;
                jumpHeight = 12;
                if (hp < 45)
                {
                    hp += 15;
                    print("HP increased by buff: " + hp);
                }
                chooseAttack(); return;
            }
            else if (currBuff == 2 && !cloneSpawned) StartCoroutine(doubleDown());
            else if (currBuff == 2 && cloneSpawned)
            {
                if (hp < 45)
                {
                    hp += 15;
                    print("HP increased by buff: " + hp);
                }
            }
            else if (currBuff == 3)
            {
                movementScript.StartCoroutine(movementScript.fatigue());
                chooseAttack(); return;
            }
        }
        else
        {
            if (currBuff == 0)
            {
                chooseAttack();
                return;
            }
            else if (currBuff == 1)
            {
                dashForce = 375;
                fallGravity = 12;
                jumpHeight = 12;
                hp += 10;
                print("HP increased by buff: " + hp);
                chooseAttack(); return;
            }
            else if (currBuff == 2 && !cloneSpawned) StartCoroutine(doubleDown());
            else if (currBuff == 2 && cloneSpawned)
            {
                hp += 10;
            }
            else if (currBuff == 3)
            {
                movementScript.StartCoroutine(movementScript.fatigue());
                chooseAttack(); return;
            }
        }

        /*
        print("HP before handling buffs: " + hp);
        if (currBuff == 0)
        {
            chooseAttack();
            return;
        }
        else if (currBuff == 1)
        {
            jumpHeight = 12;
            if (Currentscene == "BuffedDiceBOSS")
            {
                dashForce = 385;
                fallGravity = 12;
                if (hp > 45)
                {
                    hp += 15;
                    print("HP increased by buff: " + hp);
                }
            }
            else
            {
                dashForce = 375;
                fallGravity = 12;
                hp += 10;
                print("HP increased by buff: " + hp);
            }
            chooseAttack(); return;
        }
        else if (currBuff == 2 && !cloneSpawned) StartCoroutine(doubleDown());
        else if (currBuff == 3)
        {
            movementScript.StartCoroutine(movementScript.fatigue());
            chooseAttack(); return;
        }
        */
    }

    private IEnumerator jumpAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        jumpTargetPos = new Vector2(player.transform.position.x, jumpHeight);
        isJumping = true;
        audioManager.playAudio(sounds[6]);
        bc.enabled = false;
        rb.gravityScale = fallGravity;
        yield return new WaitForSeconds(0.5f);
        spriteAnim.Play("bossSpin");
        rb.MovePosition(jumpTargetPos);
        yield return new WaitForSeconds(0.35f);
        bc.enabled = true;
        isJumping = false;
        yield return new WaitForSeconds(0.35f);
        rb.gravityScale = 1;
        isAttacking = false;
    }

    private IEnumerator deathCRT()
    {
        movementScript.isInvulnerable = true;
        dead = true;
        audioManager.stopAudio(sounds[1]);
        audioManager.stopAudio(sounds[0]);
        if (!isClone) audioManager.playAudio(sounds[7]);
        yield return new WaitForSeconds(1.7f);
        movementScript.isInvulnerable = true;
        shakeCam.SetActive(true);
        if (!isClone) audioManager.playAudio(sounds[7]);
        yield return new WaitForSeconds(1f);
        if (!isClone) audioManager.playAudio(sounds[7]);
        yield return new WaitForSeconds(0.3f);
        if (!isClone) audioManager.playAudio(sounds[7]);
        yield return new WaitForSeconds(0.6f);
        if (!isClone) audioManager.playAudio(sounds[7]);
        deathParticles.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        if (!isClone) audioManager.playAudio(sounds[7]);
        if (!isClone) fadeOutAnim.Play("sceneFadeOut");
        yield return new WaitForSeconds(1f);
        if (!isClone) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator dashAttack()
    {
        isAttacking = true;
        spriteAnim.Play("bossDash");
        yield return new WaitForSeconds(0.7f);
        rb.gravityScale = 9;
        audioManager.playAudio(sounds[5]);
        rb.AddForce(dashForce * 1000 * transform.right * -playerDir);
        yield return new WaitForSeconds(1.5f);
        rb.gravityScale = 1;
        isAttacking = false;
    }

    private IEnumerator doubleDown()
    {
        if (isDoubleDownActive) yield break;

        isDoubleDownActive = true;
        isAttacking = true;
        jumpTargetPos = new Vector2(player.transform.position.x, jumpHeight + 20);
        if (Currentscene == "DiceBOSSBackUp")
        {
            if (hp < 28) hp = 28;
        }
        else
        {
            if (hp < 21) hp = 21;
        }
        duo = Instantiate(clone, jumpTargetPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        isAttacking = false;
        isDoubleDownActive = false;
        chooseAttack();
    }

    private IEnumerator cloneSpawn()
    {
        yield return new WaitForEndOfFrame();
        gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 504;
        isAttacking = true;
        StartCoroutine(jumpAttack());
    }

    private IEnumerator screenShake()
    {
        audioManager.playAudio(sounds[UnityEngine.Random.Range(2, 4)]);
        shakeCam.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        shakeCam.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking) StartCoroutine(screenShake());
        if (collision.collider.CompareTag("ground")) isJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("playerSlash") && !isInvulnerable)
        {
            audioManager.playAudio(sounds[4]);
            print("HP before damage: " + hp);
            hp -= 1;
            print("HP after damage: " + hp);
            spriteAnim.Play("damage");
        }
        if (collision.CompareTag("HeavyAttack") && !isInvulnerable)
        {
            audioManager.playAudio(sounds[4]);
            print("HP before damage: " + hp);
            hp -= 3;
            print("HP after damage: " + hp);
            spriteAnim.Play("damage");
        }
        if (collision.CompareTag("dBossFailSafe"))
        {
            StartCoroutine(jumpAttack());
        }
        if (collision.CompareTag("LHAttack") && !isInvulnerable)
        {
            audioManager.playAudio(sounds[4]);
            print("HP before damage: " + hp);
            hp -= 3;
            print("HP after damage: " + hp);
            spriteAnim.Play("damage");
        }
    }
}