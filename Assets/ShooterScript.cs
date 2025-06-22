using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterScript : MonoBehaviour
{
    [SerializeField]
    private Transform[] shooters;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private float rotationSpeed, shootingSpeed;

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotationSpeed);
    }


    public void startShooting()
    {
        StartCoroutine(shooting());
    }

    public void stopShooting()
    {
        StopAllCoroutines();
        StopCoroutine(shooting());
    }

    private IEnumerator shooting()
    {
        if(Random.Range(0, 7) == 0) rotationSpeed = -rotationSpeed;
        foreach (Transform shooter in shooters)
        {
            Instantiate(bullet, shooter.position, shooter.rotation);
        }
        yield return new WaitForSeconds(shootingSpeed);
        StartCoroutine(shooting());
    }
}
