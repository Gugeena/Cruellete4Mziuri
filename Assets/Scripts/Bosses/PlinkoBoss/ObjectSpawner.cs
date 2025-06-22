using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject obj;
    public float time, killTime;
    public bool onAwake = true;
    public bool killSelf = false;
    public Transform parent;
    private void Awake()
    {
        if(onAwake)StartCoroutine(spawner());
    }
    public void startSpawning()
    {
        StartCoroutine(spawner());
    }

    public void stopSpawning()
    {
        StopAllCoroutines();
    }
    private IEnumerator spawner()
    {
        GameObject o;
        if (parent != null)o = Instantiate(obj, transform.position, Quaternion.identity, parent);
        else o = Instantiate(obj, transform.position, Quaternion.identity);
        if(killSelf) Destroy(o, killTime);
        yield return new WaitForSeconds(time);
        StartCoroutine(spawner());
    }
}
