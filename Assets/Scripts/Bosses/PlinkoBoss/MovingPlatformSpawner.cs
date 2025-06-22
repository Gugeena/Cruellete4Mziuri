using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSpawner : MonoBehaviour
{
    public GameObject movingPlatform;
    public float time;
    void Start()
    {
        StartCoroutine(spawner());
    }
    private IEnumerator spawner()
    {
        Instantiate(movingPlatform, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(time);
        StartCoroutine(spawner());
    }

}
