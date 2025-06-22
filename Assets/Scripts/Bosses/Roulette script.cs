using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Roulettescript : MonoBehaviour
{
    /*
    public GameObject indicator;
    public float spinSpeed;
    public float spinTime;
    public Collider2D IndicatorCollider;
    */
    public int bossState;
    void Start()
    {
        //StartCoroutine(Spiner());
        StartCoroutine(amomrcheveli());
        bossState = 1000;
    }

    void Update()
    {
    }

    private IEnumerator amomrcheveli()
    {
        //animplay
        yield return new WaitForSeconds(0f);
        int decision = Random.Range(0, 3);
        if (decision == 1) bossState = 1;
        else if (decision == 2) bossState = 2;
        else if (decision == 0) bossState = 0;
        Debug.Log($"BossState updated: Decision={decision}, BossState={bossState}");
    }

    /*
    private IEnumerator Spiner()
    {
        /*
        spinSpeed = Random.Range(500f, 1000f);
        spinTime = Random.Range(3f, 7f);
        float elapsedTime = 0f;

        while (elapsedTime < spinTime)
        {
            float rotationThisTime = spinSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotationThisTime);
            spinSpeed = Mathf.Max(0, spinSpeed - (360f / spinTime) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spinSpeed = 0;
        IndicatorCollider.enabled = true;
     }
    */

}
