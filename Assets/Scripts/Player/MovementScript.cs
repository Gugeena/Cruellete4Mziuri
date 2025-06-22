using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.AI;
using UnityEngine.SceneManagement;

public class MovementScript : MonoBehaviour
{
    Rigidbody2D rb;

    public float speed;
    public float jumpForce, jumpStorageTime, fallGravity, gravity;
    bool isGrounded;
    bool jumpStored;
    int direction;

    bool isFatigued;
    public GameObject fatigueParticles;

    bool hasDashed, isDashing, dashCoolDown;
    public float dashForce;


    public GameObject SideAttackGO, TopAttackGO, BotAttackGO, HeavyAttackGO, LightHeavyAttackGO;
    bool isAttacking, isAttackCooldown;
    public float attackCooldown = 0.22f;

    int hp = 5;
    public Animator[] HPanims;


    Animator anim;

    public bool isInvulnerable;
    public float invulTime;

    public GameObject shakeCam;
    public Animator fadeOutAnim;

    public static bool dead;

    public AudioManager am;
    public AudioSource[] audioSources;

    bool devMode = false;

    bool isOnRoulette;
    public GameObject rouletteSpin;
    public KeyBindManager kbm;

    public static KeyCode dashKey1;
    public static KeyCode attackKey1;
    public static KeyCode jumpKey1;
    public static KeyCode HeavyKey1;

    Vector2 platformVelocity = Vector2.zero;

    public GameObject DeathText, DeathButton, DeathButton1;

    [SerializeField] private AudioMixer audioMixer;
    public GameObject DiceBoss;
    public GameObject SlotBoss;
    public GameObject PlinkoBoss;
    public GameObject DiceBossDuo;

    private bool hasgoneforsalt = false;

    public static int DeathCount = 0;
    public static float TimeP = 0;
    public static int Hit = 0;

    Scene activeScene;
    string currentSceneName;

    [SerializeField] private Boolean IsHeavyAttacking;

    bool onMovePlat;
    Rigidbody2D platformRB;

    public GameObject audioManager;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene();
        lockCursor();


        StartCoroutine(UpdateTime());

        dashKey1 = KeyBindManager.dashKey;
        attackKey1 = KeyBindManager.attackKey;
        jumpKey1 = KeyBindManager.jumpKey;
        HeavyKey1 = KeyBindManager.heavyKey;

        devMode = false;
        dead = false;
        isAttackCooldown = false;
        IsHeavyAttacking = false;

        if (activeScene.name == "BuffedDiceBOSS" || activeScene.name == "BuffedPlinkoBoss" || activeScene.name == "BuffedSlotMachine")
        {
            hp = 1;
        }
        else
        {
            hp = 5;
        }

        rb = GetComponent<Rigidbody2D>();
        direction = 1;
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();

        fatigueParticles.GetComponent<ParticleSystem>().Stop();
    }

    void Update()
    {
        try
        {
            if (currentSceneName == "DiceBOSSBackup") DiceBossDuo = GameObject.Find("diceBossDuo(Clone)");
            if (currentSceneName == "BuffedDiceBOSS") DiceBossDuo = GameObject.Find("diceBossDuoBuffed(Clone)");
            if (DiceBossDuo == null)
            {
                throw new System.Exception("diceboss duo not found");
            }
        } 
        catch (System.Exception e)
        {
            DiceBossDuo = null;
            Debug.Log("warning: " + e);
        }

        if (activeScene.name == "MainMenu")
        {
            DeathCount = 0;
            TimeP = 0;
            Hit = 0;
        }

        if (hasgoneforsalt)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (activeScene.name == "BuffedPlinkoBoss" || activeScene.name == "BuffedSlotBoss")
                {
                    SceneManager.LoadScene(8);
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }    

        if (hp > 0)
        {
            handleMovement();
            handleAttacks();
            handleDashing();
        }
        else if (!dead)
        {
            StartCoroutine(death());
        }

        if (Input.GetKey(KeyCode.PageDown) && Input.GetKeyDown(KeyCode.PageUp))
        {
            print("devmode: " + devMode);
            devMode = !devMode;
        }
    }

    private void LateUpdate()
    {
        if (isOnRoulette)
        { 
            transform.eulerAngles = Vector3.zero;
        }
    }

    void handleMovement()
    {
        //Horizontal
        float x = Input.GetAxisRaw("Horizontal");
        if ((!isDashing && !IsHeavyAttacking && !onMovePlat) || (IsHeavyAttacking && !isGrounded)) rb.velocity = new Vector2(x * speed, rb.velocity.y);
        else if (onMovePlat && !isDashing && !IsHeavyAttacking)
        {
            rb.velocity = new Vector2(x * speed + platformRB.velocity.x, rb.velocity.y);
        }
        else if (onMovePlat && IsHeavyAttacking) 
        {
            rb.velocity = new Vector2(platformRB.velocity.x, rb.velocity.y);
        }
        /*
        else if (onMovePlat && !isDashing)
        {
            rb.velocity = new Vector2(x * speed + platformRB.velocity.x, rb.velocity.y);
        }
        */

        if (x != 0) anim.SetInteger("state", 1);
        else anim.SetInteger("state", 0);

        if(isGrounded && IsHeavyAttacking && !onMovePlat)
        {
            rb.velocity = rb.velocity * 0;
        }


        if (x < 0 && transform.localScale.x > 0 && !isDashing && !IsHeavyAttacking || x < 0 && transform.localScale.x > 0 && !isGrounded)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direction = -1;
        }
        else if (x > 0 && transform.localScale.x < 0 && !isDashing && !IsHeavyAttacking || x > 0 && transform.localScale.x < 0 && !isGrounded)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            direction = 1;
        }
        /*
        //Flipping the player and setting the direction variable
        if (x < 0 && transform.localScale.x > 0 && !isDashing || x < 0 && transform.localScale.x > 0 && !isGrounded && !IsHeavyAttacking)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); 
            direction = -1;

        }

        else if (x > 0 && transform.localScale.x < 0 && !isDashing || x > 0 && transform.localScale.x < 0 && !isGrounded && !IsHeavyAttacking)
        { 
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); 
            direction = 1; 
        }
        */

        //Jumping
        if (Input.GetKeyDown(jumpKey1) && isGrounded && !IsHeavyAttacking)
        {
            Jump();
        }

        
        //Jump Storing
        else if (Input.GetKeyDown(jumpKey1) && !isGrounded) 
        {
            jumpStored = true;
        }
        if (Input.GetKeyUp(jumpKey1) && jumpStored) 
        {
            jumpStored = false;
        }

        //Fall gravity change
        if (rb.velocity.y < 0 && !isDashing && !isOnRoulette) rb.gravityScale = fallGravity;
        else if(rb.velocity.y >= 0 && !isDashing && !isOnRoulette) rb.gravityScale = gravity;

        if (rb.velocity.y < -10) anim.SetInteger("jumpState", -1);
        else if (rb.velocity.y > 10) anim.SetInteger("jumpState", 1);
        else anim.SetInteger("jumpState", 0);
    }

    void Jump()
    {
        if (!isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            if(am != null)am.playAudio(audioSources[0]);
        }
    }

    void handleDashing()
    {
        if(((Input.GetKeyDown(dashKey1) && !hasDashed) || (Input.GetKeyDown(dashKey1) && isGrounded)) && !isDashing && !isFatigued && !dashCoolDown && !IsHeavyAttacking)
        {
            StartCoroutine(dash());
        }
    }


    void handleAttacks()
    {
        if (!isAttackCooldown && !isAttacking && !isDashing) 
        {
            float y = Input.GetAxisRaw("Vertical");
            if (Input.GetKeyDown(attackKey1) && y > 0)
            {
                anim.SetInteger("attackDirection", 1);
                anim.Play("upSlash");
                StartCoroutine(slashAttack(TopAttackGO));
            }
            else if (Input.GetKeyDown(attackKey1) && y < 0 && !isGrounded)
            {
                anim.SetInteger("attackDirection", -1);
                anim.Play("DownSlash");
                StartCoroutine(slashAttack(BotAttackGO));
            }
            else if (Input.GetKeyDown(attackKey1) && y == 0)
            {
                anim.SetInteger("attackDirection", 0);
                anim.Play("SideSlash");
                StartCoroutine(slashAttack(SideAttackGO));
            }
        }

        if(!isAttackCooldown && !isAttacking && !isDashing && !IsHeavyAttacking && !isFatigued)
        {
            if(Input.GetKeyDown(HeavyKey1))
            {
                StartCoroutine(HeavyAttack());
            }
        }

    }
    public IEnumerator HeavyAttack()
    {
        if (!isGrounded)
        {
            isAttackCooldown = true;
            IsHeavyAttacking = true;
            //anim.SetBool("isHeavyAttacking", true);
            anim.Play("HeavyAttack");

            float attackTime = 0.75f;

            yield return new WaitForSeconds(attackTime);
            if (!IsHeavyAttacking) yield break;
            //am.playAudio(audioSources[7]);
            HeavyAttackGO.SetActive(true);
            yield return new WaitForSeconds(0.12f);

            //anim.SetBool("isHeavyAttacking", false);
            HeavyAttackGO.SetActive(false);
            IsHeavyAttacking = false;
            isAttackCooldown = false;
        }
        else
        {
            isAttackCooldown = true;
            IsHeavyAttacking = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            //anim.SetBool("isLightHeavyAttacking", true);
            anim.Play("LightHeavyAttack");

            float attackTime = 0.45f;

            yield return new WaitForSeconds(attackTime);
            if (!IsHeavyAttacking) yield break;
            //am.playAudio(audioSources[7]);
            //LightHeavyAttackGO.SetActive(true);
            HeavyAttackGO.SetActive(true);
            yield return new WaitForSeconds(0.12f);

            //anim.SetBool("isLightHeavyAttacking", false);
            //LightHeavyAttackGO.SetActive(false);
            HeavyAttackGO.SetActive(false);
            yield return new WaitForSeconds(0.18f); 
            IsHeavyAttacking = false;
            isAttackCooldown = false;
        }
    }

    public IEnumerator fatigue()
    {
        isFatigued = true;
        fatigueParticles.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(8f);
        isFatigued = false;
        fatigueParticles.GetComponent<ParticleSystem>().Stop();
    }
    private IEnumerator dash()
    {
        dashCoolDown = true;
        hasDashed = true;
        isDashing = true;
        if (am != null) am.playAudio(audioSources[1]);
        anim.SetBool("isDashing", true);
        anim.Play("playerDash");
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0;
        rb.AddForce(transform.right * dashForce * direction * 100);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isDashing", false);
        isDashing = false;
        rb.gravityScale = gravity;
        yield return new WaitForSeconds(0.3f);
        dashCoolDown = false;
    }

    private IEnumerator damage()
    {
        if (!isInvulnerable && !devMode)
        {
            if (am != null) am.playAudio(audioSources[3]);
            anim.Play("Damage");
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0.1f);
            Time.timeScale = 1;
            HPanims[hp - 1].Play("ui_loseHP");
            hp--;
            Hit++;
            print("Hit:" + Hit);

            if(IsHeavyAttacking)
            {
                StopCoroutine(HeavyAttack());
                HeavyAttackGO.SetActive(false);
                LightHeavyAttackGO.SetActive(false);
                IsHeavyAttacking = false;
                isAttackCooldown = false;
                anim.SetBool("IsHeavyAttacking", false);
                //anim.SetBool("IsLightHeavyAttacking", false);
                if (isGrounded) anim.Play("PlayerIdleAnim");
                else if (!isGrounded && rb.velocity.y > 0) anim.Play("JumpUp");
                else if (!isGrounded && rb.velocity.y < 0) anim.Play("JumpDown");
                print("Heavy attacked interfered");
            }

            isInvulnerable = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(invulTime);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            isInvulnerable = false;
        }
    }

    private IEnumerator death()
    {
        if (am != null) am.playAudio(audioSources[2]);
        dead = true;
        shakeCam.SetActive(true);
        fadeOutAnim.Play("sceneFadeOut");
        //audioMixer.SetFloat("music", -80);
        //audioMixer.SetFloat("sfxVolume", -80);
        audioManager.SetActive(false);
        yield return new WaitForSecondsRealtime(1.2f);
        bossDeactivation();
        hasgoneforsalt = true;
        unlockCursor();
        yield return new WaitForSecondsRealtime(0.2f);
        DeathButton.SetActive(true);
        DeathButton1.SetActive(true);
        DeathText.SetActive(true);
        DeathCount++;
        print("Death:" + DeathCount);
    }

    private void bossDeactivation()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "DiceBOSSBackup" || currentSceneName == "BuffedDiceBOSS")
        {
            DiceBoss.SetActive(false);
            if(DiceBossDuo != null) DiceBossDuo.SetActive(false);
            isInvulnerable = true;
            audioSources[3] = null;
        }
        else if (currentSceneName == "PlinkoBoss" || currentSceneName == "BuffedPlinkoBoss")
        {
            PlinkoBoss.SetActive(false);
            isInvulnerable = true;
            audioSources[3] = null;
        }
        else if (currentSceneName == "BossSlotMachine")
        {
            SlotBoss.SetActive(false);
            isInvulnerable = true;
            audioSources[3] = null;
        }
    }

    private IEnumerator slashAttack(GameObject slash)
    {
        if (am != null) am.playAudio(audioSources[UnityEngine.Random.Range(4, 7)]);
        isAttacking = true;
        anim.SetBool("isAttacking", true);
        slash.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        slash.SetActive(false);
        isAttacking = false;
        anim.SetBool("isAttacking", false);
        isAttackCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttackCooldown = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("mp"))
        {
            onMovePlat = true;
            platformRB = collision.gameObject.GetComponent<Rigidbody2D>();
        }

        if (collision.gameObject.layer == 3)
        {
            if(jumpStored){Jump(); jumpStored = false;}
            
        }


        if ((collision.gameObject.layer == 7 || collision.gameObject.layer == 6 || collision.gameObject.CompareTag("Enemy")))
        {
            StartCoroutine(damage());
        }
        if (collision.gameObject.CompareTag("damageFloor"))
        {
            StartCoroutine(damage());
            Jump();
            jumpStored = false;
            anim.SetBool("airborne", true);
        }

        //if (collision.gameObject.CompareTag("roulettePlatform"))
        //{
        //    isOnRoulette = true;
        //    anim.SetBool("airborne", false);
        //    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        //    transform.parent = rouletteSpin.transform;
        //    rb.gravityScale = 0;
        //}

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && !isDashing)
        {
            hasDashed = false;
            isGrounded = true;
            anim.SetBool("airborne", false);
        }
        //if (collision.gameObject.CompareTag("roulettePlatformExit"))
        //{
        //    hasDashed = false;
        //    isGrounded = true;
        //}

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("mp"))
        {
            onMovePlat = false;
            platformRB = null;

        }

        if (collision.gameObject.layer == 3)
        {
            isGrounded = false;
            anim.SetBool("airborne", true);
        }

        //if (collision.gameObject.CompareTag("mp"))
        //{
          //  rb.velocity -= platformVelocity; // Remove extra velocity when leaving platform
           // currentPlatform = null;
           // platformVelocity = Vector2.zero;
        //}
        /*
        if (collision.gameObject.tag == "mp") 
        {
            transform.parent = null;
            rb.isKinematic = false;
        }
        */
        //if (collision.gameObject.CompareTag("roulettePlatformExit"))
        //{
        //    isOnRoulette = false;
        //    anim.SetBool("airborne", true);
        //    transform.parent = null;
        //    rb.gravityScale = gravity;
        //}
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("roulettePlatformExit"))
        //{
        //    isOnRoulette = false;
        //    anim.SetBool("airborne", true);
        //    transform.parent = null;
        //    rb.gravityScale = gravity;

        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 7 || collision.gameObject.layer == 6 || collision.gameObject.CompareTag("Enemy")) && !isInvulnerable && !devMode)
        {
            StartCoroutine(damage());
        }
        //if (collision.gameObject.CompareTag("roulettePlatform"))
        //{
        //    isOnRoulette = true;
        //    anim.SetBool("airborne", false);
        //    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        //    transform.parent = rouletteSpin.transform;
        //    rb.gravityScale = 0;
        //}
    }

    public IEnumerator UpdateTime()
    {
        //Debug.Log("UpdateTime started");

        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (SceneManager.GetActiveScene().name == "EndScene")
            {
                //Debug.Log("Timer stopped at EndScene");
                break;
            }

            TimeP++;
        }
    }

    public void unlockCursor() 
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void lockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}