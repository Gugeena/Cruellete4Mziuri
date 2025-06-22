using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonteScript : MonoBehaviour
{
    public GameObject[] WhatToSpawn;
    public Transform[] SpawnLocation;
    public GameObject[] WhatToSpawnClone;

    void Start()
    {
        StartCoroutine(ExplosiveCardAttack());
    }

    void Update()
    {
        
    }

    private IEnumerator ExplosiveCardAttack()
    {
        int[] spawnIndices = { 0, 1, 2, 3};

        foreach (int i in spawnIndices)
        {
            Instantiate(WhatToSpawn[0], SpawnLocation[i].transform.position, Quaternion.identity);
        }
        yield return null;
    }
}
