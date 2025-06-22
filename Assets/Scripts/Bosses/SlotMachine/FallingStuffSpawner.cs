using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStuffSpawner : MonoBehaviour
{
    public GameObject[] stuff;
    public GameObject rightBound, leftBound;
    public bool isEnabled = true;
    void Start()
    {
        StartCoroutine(spawner());
    }


    private IEnumerator spawner()
    {
        if (isEnabled)
        {
            yield return new WaitForSeconds(1.4f);
            Vector2 pos = new Vector2(Random.Range(leftBound.transform.position.x, rightBound.transform.position.x), leftBound.transform.position.y);
            pos.x += Random.Range(-5f, 5f);
            Instantiate(stuff[Random.Range(0, stuff.Length)], pos, Quaternion.identity);
            StartCoroutine(spawner());
        }
    }
}
