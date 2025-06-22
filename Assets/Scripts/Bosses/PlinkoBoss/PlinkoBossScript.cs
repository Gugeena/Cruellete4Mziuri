using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlinkoBossScript : MonoBehaviour
{

    public int state;
    int reload;

    public Slider hpSlider;
    float hpCurVel = 0;

    public GameObject Orb;
    public GameObject FlyEnemy;
    public GameObject Particles;
    public GameObject Particles1;
    public GameObject[] OrbSpawners;
    public static int hp = 26;
    public GameObject[] WhatToSpawn;
    public Transform[] SpawnLocation;
    public GameObject[] WhatToSpawnClone;
    public Transform[] BuffProjectileSpawner;
    public bool CanAttack;
    public PlinkoProjectileCode ppc;
    

    public rouletteScript rs;
    bool spinQueued;

    bool dead;

    public AudioManager am;
    public AudioSource[] sounds;
    double nextEventTime;
    public AudioLowPassFilter lowPass;

    public MovementScript movementScript;
    public GameObject shakeCam;
    public GameObject deathParticles;
    public Animator fadeOutAnim;

    public int currBuff;
    public Sprite[] toastSprites;

    public GameObject movePlatformSpawner;
    public Scene activeScene;

    public bool chancesRaised;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene();

        movePlatformSpawner.SetActive(false);
        MovingPlatformScript.falling = false;
        CanAttack = false;
        MovingPlatformScript.speedX = 0;
        if(activeScene.name == "BuffedPlinkoBoss")
        {
            hp = 35;
            spinQueued = true;
        }
        else 
        {
            hp = 26;
        }
        ppc = FindObjectOfType<PlinkoProjectileCode>();

        sounds[1].clip.LoadAudioData();
        nextEventTime = AudioSettings.dspTime + sounds[0].clip.length;
        am.playScheduled(sounds[1], nextEventTime);

        lowPass = GetComponent<AudioLowPassFilter>();

        StartCoroutine(fightStart());
    }

    void Update()
    {
        if (!spinQueued)
        {
            int decision = Random.Range(0, 9);
            if (chancesRaised && decision <= 5 && CanAttack&& !isPerformingPredeterminedAttack || !chancesRaised && decision <= 7 && CanAttack && !isPerformingPredeterminedAttack)
            {
                CanAttack = false;
                if (state == 1 && reload <= 0)
                {

                    Instantiate(Orb, OrbSpawners[Random.Range(0, OrbSpawners.Length)].transform.position, Quaternion.identity);
                    reload = Random.Range(150, 200);
                    CanAttack = true;
                }
            }
            else if (chancesRaised && decision <= 6 && CanAttack && !isPerformingPredeterminedAttack || !chancesRaised && decision == 8 && CanAttack && !isPerformingPredeterminedAttack)
            {
                CanAttack = false;
                StartCoroutine(PreDeterminedAttack());
            }
        }
        else
        {
                rs.StartCoroutine(rs.spinCRT(toastSprites, 5));
                spinQueued = false;
        }

        handleMusic();
        handleHP();

        print("ChancesRaised: " + chancesRaised);
    }


    void handleMusic()
    {

        if (PauseMenu.gamePaused) lowPass.enabled = true;
        else lowPass.enabled = false;
    }

    private bool isPerformingPredeterminedAttack = false; 

    private IEnumerator PreDeterminedAttack()
    {
        isPerformingPredeterminedAttack = true; 

        CanAttack = false;

        int[] spawnIndices = { 0, 1, 2, 3 };

        foreach (int i in spawnIndices)
        {
            Instantiate(WhatToSpawn[0], SpawnLocation[i].transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);
        CanAttack = true;

        isPerformingPredeterminedAttack = false;
    }

    void handleHP()
    {
        hp = Mathf.Clamp(hp, 0, 35);
        float currHP = Mathf.SmoothDamp(hpSlider.value, hp, ref hpCurVel, 0.2f);
        hpSlider.value = currHP;
        if (activeScene.name == "BuffedPlinkoBoss")
        {
            if ((hp <= 25 && rs.spinCount <= 1) || (hp <= 15 && rs.spinCount <= 2) || (hp <= 7 && rs.spinCount <= 3))
            {
                spinQueued = true;
            }
        }
        else
        {
            if ((hp <= 20 && rs.spinCount <= 0) || (hp <= 13 && rs.spinCount <= 1) || (hp <= 7 && rs.spinCount <= 2))
            {
                spinQueued = true;
            }
        }

        if (hp <= 0 && !dead) StartCoroutine(deathCRT());
    }

    private IEnumerator deathCRT()
    {
        movementScript.isInvulnerable = true;
        MovingPlatformScript.speedX = MovingPlatformScript.speedY = 0;
        dead = true;
        am.stopAudio(sounds[1]);
        am.playAudio(sounds[2]);
        yield return new WaitForSeconds(2.5f);
        shakeCam.SetActive(true);
        deathParticles.SetActive(true);
        am.playAudio(sounds[2]);
        yield return new WaitForSeconds(1f);
        am.playAudio(sounds[2]);
        yield return new WaitForSeconds(0.3f);
        am.playAudio(sounds[2]);
        yield return new WaitForSeconds(0.6f);
        am.playAudio(sounds[2]);
        deathParticles.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        am.playAudio(sounds[2]);
        yield return new WaitForSeconds(0.5f);
        am.playAudio(sounds[2]);
        fadeOutAnim.Play("sceneFadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator fightStart()
    {
        MovingPlatformScript.speedX = 0;
        yield return new WaitForSeconds(1.5f);
        MovingPlatformScript.speedX = 0.3f;
        CanAttack = true;
        movePlatformSpawner.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (reload > 0)
        {
            reload--;
        }
        else
        {
            CanAttack = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Orb")
        {
            hp--;
            Destroy(collision.gameObject);
            hpSlider.value--;
            Debug.Log(hpSlider.value);
        }
    }

    public void handleBuffs()
    {
        MovingPlatformScript.falling = false;
        if (currBuff == 0) //AgainstOdds
        {
            return;
        }
        else if (currBuff == 1) //Frenzy
        {
            StartCoroutine(frenzyAttack());
        }
        else if (currBuff == 2) //Cmoon
        {
            StartCoroutine(cmoonAttack());
        }
        else if (currBuff == 3) // Fatigue
        {
            movementScript.StartCoroutine(movementScript.fatigue());
        }
        else if (currBuff == 4) //Buff
        {
            if (activeScene.name == "BuffedPlinkoBoss")
            {
                if (hp < 31) hp += 4;
                StartCoroutine(ChanceRaising());
            }
            else hp += 4;
        }
    }

    private IEnumerator cmoonAttack()
    {
        if (activeScene.name == "BuffedPlinkoBoss")
        {
            MovingPlatformScript.falling = true;
            yield return new WaitForSeconds(10f);
            MovingPlatformScript.falling = false;
        }
        else
        {
            MovingPlatformScript.falling = true;
            yield return new WaitForSeconds(8f);
            MovingPlatformScript.falling = false;
        }
    }

    private IEnumerator ChanceRaising()
    {
        chancesRaised = true;
        yield return new WaitForSeconds(10f);
        chancesRaised = false;
    }

    private IEnumerator frenzyAttack()
    {
        if (activeScene.name == "BuffedPlinkoBoss")
        {
            Debug.Log("30");
            for (int i = 0; i < 30; i++)
            {
                int randomSpawnerIndex = Random.Range(0, OrbSpawners.Length);

                Instantiate(WhatToSpawnClone[0], OrbSpawners[randomSpawnerIndex].transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.4f);
            }
        }
        else
        {
            Debug.Log("20");
            for (int i = 0; i < 20; i++)
            {
                int randomSpawnerIndex = Random.Range(0, OrbSpawners.Length);

                Instantiate(WhatToSpawnClone[0], OrbSpawners[randomSpawnerIndex].transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.4f);
            }      
        }
    }
}