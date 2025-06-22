using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerSpawningAnimationScript : MonoBehaviour
{
    public float scaleFactor = 0.5f; // Factor to scale down (50% of current size)
    public float slamDuration = 0.3f;

    private Vector3 targetScale;

    void Start()
    {
        // Calculate the target scale based on the current scale
        targetScale = transform.localScale * scaleFactor;
        StartSlam();
    }

    public void StartSlam()
    {
        StartCoroutine(SlamCoroutine());
    }

    private IEnumerator SlamCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale; // Get the current scale at the start of the slam

        while (elapsedTime < slamDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / slamDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it reaches the target scale at the end
    }
}