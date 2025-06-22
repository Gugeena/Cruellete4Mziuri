using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SlotBossAI : MonoBehaviour
{

    Rigidbody2D rb;
    public GameObject slotFace, bouncyOrange, lemonLaser, orangeSpawner, laserIndicator, cherryBomb, diceBoss, cherryParticles, jackpotPlatformSpawner, jackpotIndicator, jackpotFloor, floorSprite;
    GameObject player;
    //int state; // -1 = Idle ; 0 = lemonLaser ; 1 = bouncingCherries ; 2 = OrangeBomb ; 3 = Jackpot(Last Phase) 
    int lastAttack, nextAttack;
    bool isAttacking;
    public float idleTime;
    public bool isInvulnerable;

    Animator anim, spriteAnim;
    public Animator slotDoorAnim;
    Animator slotFaceAnim;
    public SpriteRenderer slotFaceSr;
    public Animator floorAnimator;
    public GameObject frontWall;


    float hpCurVel = 0f;
    public UnityEngine.UI.Slider hpSlider;
    public float hp;

    public rouletteScript rs;
    

    FallingStuffSpawner fStuffScript;

    public int currBuff;
    bool spinQueued;
    public Sprite[] toastSprites;

    MovementScript movementScript;

    public GameObject shakeCam;
    public Animator fadeOutAnim;

    public AudioManager am;
    public AudioSource[] audioSources;
    AudioLowPassFilter lowPass;

    bool dead;
    public GameObject deathParticles;

    bool musicPlaying;
    private double nextEventTime;

    private Scene currentScene;

    public GameObject diceWall; 

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        dead = false;
        player = GameObject.Find("Player");
        movementScript = player.GetComponent<MovementScript>();
        fStuffScript = GetComponent<FallingStuffSpawner>();
        fStuffScript.isEnabled = true;
        if (currentScene.name == "BuffedSlotMachine") hp = 150;
        else hp = 110;
        //state = -1;
        currBuff = -1;
        spinQueued = false;

        MovingPlatform.speedY = 0;
        MovingPlatform.speedX = 4;

        rb = GetComponent<Rigidbody2D>();
        slotFaceAnim = transform.GetChild(1).GetComponent<Animator>();
        slotFaceSr = transform.GetChild(1).GetComponent<SpriteRenderer>();
        spriteAnim = transform.GetChild(0).GetComponent<Animator>();
        anim = GetComponent<Animator>();


        StartCoroutine(Idle());

        audioSources[4].clip.LoadAudioData();
        am.playAudio(audioSources[13]);
        nextEventTime = AudioSettings.dspTime + audioSources[13].clip.length;

        lowPass = GetComponent<AudioLowPassFilter>();

        if (currentScene.name == "BuffedSlotMachine")
        {
            spinQueued = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleHP();
        handleMusic();
    }

    void handleMusic()
    {
        double time = AudioSettings.dspTime;

        if (!musicPlaying && time + 1.0f > nextEventTime && hp > 25)
        {
            musicPlaying = true;
            am.playScheduled(audioSources[4], nextEventTime);
        }

        if(!musicPlaying && hp <= 25)
        {
            musicPlaying = true;
            nextEventTime = AudioSettings.dspTime + audioSources[11].clip.length;
            am.playScheduled(audioSources[12], nextEventTime);
        }

        if (PauseMenu.gamePaused) lowPass.enabled = true;
        else lowPass.enabled = false;
    }
    void handleHP()
    {
        hp = Mathf.Clamp(hp, 0, 150);
        float currHP = Mathf.SmoothDamp(hpSlider.value, hp, ref hpCurVel, 0.2f);
        hpSlider.value = currHP;
        if (currentScene.name == "BuffedSlotMachine")
        {
            if ((hp <= 127 && rs.spinCount <= 1) || (hp <= 82 && rs.spinCount <= 2) || (hp <= 37 && rs.spinCount <= 3))
            {
                spinQueued = true;
            }
        }
        else
        {
            if ((hp <= 100 && rs.spinCount <= 0) || (hp <= 75 && rs.spinCount <= 1) || (hp <= 50 && rs.spinCount <= 2))
            {
                spinQueued = true;
            }
        }

        if(hp <= 0 && !dead) StartCoroutine(deathCRT());
    }

    public void ChooseAttack()
    {
        lastAttack = nextAttack;
        if (spinQueued)
        {
            rs.StartCoroutine(rs.spinCRT(toastSprites, 5));
            spinQueued = false;
            isInvulnerable = true;
        }
        else if (hp > 25)
        {
            nextAttack = Random.Range(0, 3);
            if (lastAttack != nextAttack)
            {
                if (nextAttack == 0)StartCoroutine(laserAttack());
                else if (nextAttack == 1)StartCoroutine(cherryAttack());
                else if (nextAttack == 2)StartCoroutine(orangeAttack());
            }
            else { ChooseAttack(); return; }
        }
        else if (hp <= 25) StartCoroutine(jackpotAttack());

        
    }

    private IEnumerator deathCRT()
    {
        movementScript.isInvulnerable = true;
        dead = true;
        am.stopAudio(audioSources[12]);
        am.playAudio(audioSources[8]);
        MovingPlatformScript.speedX = MovingPlatformScript.speedY = 0;
        fStuffScript.isEnabled = false;
        fStuffScript.enabled = false;
        yield return new WaitForSeconds(2.5f);
        anim.SetBool("isShaking", true);
        shakeCam.SetActive(true);
        am.playAudio(audioSources[8]);
        yield return new WaitForSeconds(1f);
        am.playAudio(audioSources[8]);
        yield return new WaitForSeconds(0.3f);
        am.playAudio(audioSources[8]);
        yield return new WaitForSeconds(0.6f);
        am.playAudio(audioSources[8]);
        deathParticles.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        am.playAudio(audioSources[7]);
        am.playAudio(audioSources[8]);
        yield return new WaitForSeconds(0.5f);
        am.playAudio(audioSources[8]);
        fadeOutAnim.Play("sceneFadeOut");
        yield return new WaitForSeconds(1f);
        if (currentScene.name == "BuffedSlotMachine") SceneManager.LoadScene(7);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void handleBuffs()
    {
        if (currBuff == 0) 
        { 
            ChooseAttack();
            return;
        }
        else if (currBuff == 1)
        {
            if (currentScene.name == "BuffedSlotMachine")
            {
                if (hp < 135) hp += 15;
                idleTime = 0.5f;
            }
            else
            {
                hp += 10;
                idleTime = 0.75f;
            }
            ChooseAttack(); return;
        }
        else if (currBuff == 2) StartCoroutine(diceAttack());
        else if (currBuff == 3)
        {
            movementScript.StartCoroutine(movementScript.fatigue());
            ChooseAttack(); return;
        }
        else if (currBuff == 4) StartCoroutine(badLuckAttack());

    }

    //Visual state change
    private IEnumerator visualAttackChange(string attack, bool spin)
    {
        if (spin)
        {
            spriteAnim.Play("slotSpin");
            yield return new WaitForSeconds(0.2f);
            anim.SetBool("isShaking", true);
        }
        slotFaceAnim.Play(attack);
        yield return new WaitForSeconds(1f);
        anim.SetBool("isShaking", false);

        slotFaceSr.enabled = false;
        yield return new WaitForSeconds(0.2f);
        slotFaceSr.enabled = true;
        yield return new WaitForSeconds(0.2f);
        slotFaceSr.enabled = false;
        yield return new WaitForSeconds(0.2f);
        slotFaceSr.enabled = true;
        yield return new WaitForSeconds(0.2f);
    }

    //Attacks & States
    private IEnumerator Idle()
    {
        //state = -1;
        yield return new WaitForSeconds(idleTime);
        ChooseAttack();
    }

    private IEnumerator laserAttack()
    {         
        //state = 0;
        StartCoroutine(visualAttackChange("NothingToLemon", true));
        am.playAudio(audioSources[1]);
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i <= Random.Range(4, 7); i++)
        {
            Vector2 pos = new Vector2(player.transform.position.x, player.transform.position.y + 30);
            //pos.x += Random.Range(-5f, 5f);
            am.playAudio(audioSources[6]);
            GameObject indicator = Instantiate(laserIndicator, pos, Quaternion.identity);
            for (int j = 0; j <= 5; ++j)
            {
                indicator.SetActive(!indicator.activeSelf);
                yield return new WaitForSeconds(0.2f);
            }
            Destroy(indicator);
            am.playAudio(audioSources[5]);
            Instantiate(lemonLaser, pos, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(visualAttackChange("LemonToNothing", false));
        StartCoroutine(Idle());
    }

    private IEnumerator cherryAttack()
    {
        //state = 1;
        StartCoroutine(visualAttackChange("NothingToCherry", true));
        am.playAudio(audioSources[1]);
        yield return new WaitForSeconds(1f);
        frontWall.SetActive(true);
        slotDoorAnim.Play("slotDoorOpen");
        yield return new WaitForSeconds(1.5f);
        Instantiate(bouncyOrange, orangeSpawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Instantiate(bouncyOrange, orangeSpawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        slotDoorAnim.Play("slotDoorClose");
        yield return new WaitForSeconds(1f);
        frontWall.SetActive(false);
        yield return new WaitForSeconds(4f);
        StartCoroutine(visualAttackChange("CherryToNothing", false));
        StartCoroutine(Idle());
    }

    private IEnumerator diceAttack()
    {
        //state = 1;
        StartCoroutine(visualAttackChange("NothingToCherry", true));
        am.playAudio(audioSources[1]);
        yield return new WaitForSeconds(1f);
        frontWall.SetActive(true);
        slotDoorAnim.Play("slotDoorOpen");
        yield return new WaitForSeconds(1.5f);
        GameObject dice = Instantiate(diceBoss, orangeSpawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        //slotDoorAnim.Play("slotDoorClose");
        yield return new WaitForSeconds(1f);
        //frontWall.SetActive(false);
        yield return new WaitForSeconds(8f);
        StartCoroutine(visualAttackChange("CherryToNothing", false));
        StartCoroutine(Idle());
        slotDoorAnim.Play("slotDoorClose");
        frontWall.SetActive(false);
    }
    private IEnumerator orangeAttack()
    {
        //state = 2;
        StartCoroutine(visualAttackChange("NothingToOrange", true));
        am.playAudio(audioSources[1]);
        yield return new WaitForSeconds(1f);
        Vector2 pos = new Vector2(player.transform.position.x + 3, player.transform.position.y + 9);
        Instantiate(cherryParticles, pos, Quaternion.identity);
        yield return new WaitForSeconds(0.35f);
        Instantiate(cherryBomb, pos, Quaternion.identity);
        StartCoroutine(visualAttackChange("OrangeToNothing", false));
        StartCoroutine(Idle());
    }
    private IEnumerator jackpotAttack()
    {
        //state = 3;
        isInvulnerable = true;
        StartCoroutine(visualAttackChange("NothingToJackpot", true));
        am.playAudio(audioSources[1]);
        frontWall.SetActive(true);
        am.stopAudio(audioSources[4]);
        am.playAudio(audioSources[10]);
        yield return new WaitForSeconds(1.5f);
        am.playAudio(audioSources[2]);      
        anim.SetBool("isShaking", true);
        jackpotPlatformSpawner.SetActive(true);
        MovingPlatformScript.speedX = 0.35f;
        yield return new WaitForSeconds(1.5f);
        am.playAudio(audioSources[6]);
        for (int i = 0; i <= 5; ++i)
        {
            jackpotIndicator.SetActive(!jackpotIndicator.activeSelf);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.7f);
        slotDoorAnim.Play("slotDoorbreak");
        am.playAudio(audioSources[3]);
        yield return new WaitForSeconds(0.5f);
        am.playAudio(audioSources[3]);
        yield return new WaitForSeconds(0.5f);
        am.playAudio(audioSources[3]);
        yield return new WaitForSeconds(0.5f);
        am.playAudio(audioSources[3]);
        am.playAudio(audioSources[7]);
        musicPlaying = false;
        am.playAudio(audioSources[11]);
        //StartCoroutine(musicP2());
        floorSprite.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        floorAnimator.Play("jackPotFloor");
        anim.SetBool("isShaking", false);
        isInvulnerable = false;
    }

    private IEnumerator badLuckAttack()
    {
        //state = 4;
        isInvulnerable = true;
        StartCoroutine(visualAttackChange("NothingToToughLuck", true));
        am.playAudio(audioSources[1]);
        frontWall.SetActive(true);
        slotDoorAnim.Play("slotDoorOpen");
        yield return new WaitForSeconds(1.5f);
        Instantiate(bouncyOrange, orangeSpawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        slotDoorAnim.Play("slotDoorClose");
        Vector2 pos = new Vector2(player.transform.position.x + 3, player.transform.position.y + 9);
        Instantiate(cherryParticles, pos, Quaternion.identity);
        yield return new WaitForSeconds(0.35f);
        Instantiate(cherryBomb, pos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i <= 4; i++)
        {
            Vector2 pos1 = new Vector2(player.transform.position.x, player.transform.position.y + 30);
            //pos.x += Random.Range(-5f, 5f);
            GameObject indicator = Instantiate(laserIndicator, pos1, Quaternion.identity);
            am.playAudio(audioSources[6]);
            for (int j = 0; j <= 5; ++j)
            {
                indicator.SetActive(!indicator.activeSelf);
                yield return new WaitForSeconds(0.2f);
            }
            Destroy(indicator);
            am.playAudio(audioSources[5]);
            Instantiate(lemonLaser, pos1, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
        isInvulnerable = false;
        StartCoroutine(visualAttackChange("ToughLuckToNothing", false));
        StartCoroutine(Idle());

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("playerSlash") && !isInvulnerable)
        {
            am.playAudio(audioSources[0]);
            hp -= 1;
            anim.Play("Damage");
        }

        if (collision.CompareTag("HeavyAttack") && !isInvulnerable)
        {
            am.playAudio(audioSources[0]);
            hp -= 3;
            anim.Play("Damage");
        }

        if (collision.CompareTag("LHAttack") && !isInvulnerable)
        {
            am.playAudio(audioSources[0]);
            hp -= 2;
            anim.Play("Damage");
        }
    }
}
