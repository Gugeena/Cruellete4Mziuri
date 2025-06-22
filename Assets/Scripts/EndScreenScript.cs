using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenScript : MonoBehaviour
{
    public Text death;
    public Text Time;
    public Text Hit;
    public GameObject P, S, A, B, C;

    Vector3 position = new Vector3(-3.2832F, -6.92F, 0);


    int counterState = 0; //0 - DeathCounter, 1 - TimeCounter, 2 - HitCounter, 3 - displayRank
    int minutes, seconds;

    public AudioSource countingSound;

    void Start()
    {
        StartCoroutine(deathCount(MovementScript.DeathCount));

        int totalSeconds = Mathf.FloorToInt(MovementScript.TimeP);
        minutes = totalSeconds / 60;
        seconds = totalSeconds % 60;

        death.text = "0";
        Time.text = "0:00";
        Hit.text = "0";


        Cursor.visible = true; // Restores the cursor when leaving the scene
        Cursor.lockState = CursorLockMode.None; // Freely movable again
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            counterState++;
        }


    }

    private IEnumerator deathCount(int toCount)
    {
        
        int count = 0;
        yield return new WaitForSeconds(0.5f);
        countingSound.Play();
        while (count < toCount && counterState == 0)
        {
            count++;
            death.text = "" + count;
            yield return new WaitForSeconds(0.05f);
        }
        if (counterState == 0) counterState++;
        death.text = "" + toCount;
        countingSound.Stop();
        StartCoroutine(timeCount(minutes, seconds));
    }

    private IEnumerator timeCount(int countMin, int countSec)
    {
        int min = 0;
        int sec = 0;
        countingSound.Play();
        while (min < countMin && counterState == 1)
        {

            sec++;
            if(sec >= 60)
            {
                sec = 0;
                min++;
            }
            Time.text = string.Format("{0}:{1:00}", min, sec);
            yield return new WaitForSeconds(0.0075f);
        }
        while (sec < countSec && counterState == 1) {
            sec++;
            Time.text = string.Format("{0}:{1:00}", min, sec);
            yield return new WaitForSeconds(0.0075f);
        }
        if(counterState == 1) counterState++;
        Time.text = string.Format("{0}:{1:00}", countMin, countSec);
        countingSound.Stop();
        StartCoroutine(hitCount(MovementScript.Hit));
    }

    private IEnumerator hitCount(int toCount)
    {
        
        int count = 0;
        yield return new WaitForSeconds(0.5f);
        countingSound.Play();
        while (count < toCount && counterState == 2)
            {
                count++;
                Hit.text = "" + count;
                yield return new WaitForSeconds(0.035f);
            }
        Hit.text = "" + toCount;
        countingSound.Stop();
        ranker();
    }

    public void ranker()
    {
        while (true)
        {
            if (MovementScript.Hit == 0 && MovementScript.DeathCount == 0 && MovementScript.TimeP < 300)
            {
                Instantiate(P, position, Quaternion.identity);
                break;
            }
            else if (MovementScript.DeathCount <= 2 && MovementScript.TimeP < 450)
            {
                Instantiate(S, position, Quaternion.identity);
                break;
            }
            else if (MovementScript.DeathCount <= 4 && MovementScript.TimeP < 720)
            {
                Instantiate(A, position, Quaternion.identity);
                break;
            }
            else if (MovementScript.DeathCount <= 7 && MovementScript.TimeP < 1200)
            {
                Instantiate(B, position, Quaternion.identity);
                break;
            }
            else
            {
                Instantiate(C, position, Quaternion.identity);
                break;
            }
        }

        MovementScript.Hit = 0;
        MovementScript.DeathCount = 0;
        MovementScript.TimeP = 0;
    }

}

