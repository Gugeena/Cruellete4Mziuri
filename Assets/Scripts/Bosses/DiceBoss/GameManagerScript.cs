using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public Boss0Script bossScript;
    public Boss0DuoScript duoScript;
    public GameObject boss;
    public GameObject duo;
    public int index = 0;
    public bool counter = false;
    private bool bossDead = false;
    private bool duoDead = false;
    public Roulettescript rouletteScript;

    private void Update()
    {
        if(rouletteScript.bossState == 0)
        {
            if (bossScript.hp <= 0 && !bossDead)
            {
                boss.SetActive(false);
                index++;
                bossDead = true;
            }

            if (duoScript.hp <= 0 && !duoDead)
            {
                duo.SetActive(false);
                index++;
                duoDead = true;
            }

            if (index > 2)
            {
                index = 2;
            }

            if (index == 2)
            {
                Debug.Log("victory");
                nextLevel();
            }
        }
        else
        {
            if (bossScript.hp <= 0)
            {
                boss.SetActive(false);
                index++;
                bossDead = true;
                nextLevel();
            }
        }
    }

    private void nextLevel()
    {
        //levelis chatvirtva
    }

    /*
    private void HandleDeathConditions()
    {
        if (bossScript.hp <= 0 && duoScript.hp <= 0)
        {
            Debug.Log("Both Boss and Duo are dead. Fight ends!");
            StartCoroutine(EndFightAndTransition());
        }
        else if (bossScript.hp <= 0)
        {
            Debug.Log("Boss has died. Player fights Duo 1v1!");
            bossScript.gameObject.SetActive(false);
            boss.SetActive(false);
        }
        else if (duoScript.hp <= 0)
        {
            Debug.Log("Duo has died. Player fights Boss 1v1!");
            duoScript.gameObject.SetActive(false);
            duo.SetActive(false);
        }
    }

    private IEnumerator EndFightAndTransition()
    {
        Debug.Log("Victory achieved");

        yield return new WaitForSeconds(2f);

        if (duoScript != null)
        {
            duo.SetActive(false);
            Debug.Log("Duo deactivated.");
        }

        if (boss != null)
        {
            boss.SetActive(false);
            Debug.Log("Boss deactivated.");
        }

        StartCoroutine(TransitionToNextLevel());
    }

    private IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Transitioning to the next level");
    }
    */
}
