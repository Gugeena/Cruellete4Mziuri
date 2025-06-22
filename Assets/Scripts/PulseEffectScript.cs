using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PulseEffectScript : MonoBehaviour
{
    private Renderer objectRenderer;
    private Vector3 originalScale;

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f; 

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scaleFactor;
    }

    private IEnumerator FadeIn()
    {
        Color initialColor = objectRenderer.material.color;
        initialColor.a = 0f;
        objectRenderer.material.color = initialColor;

        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            initialColor.a = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            objectRenderer.material.color = initialColor;
            yield return null;
        }

        initialColor.a = 1f;
        objectRenderer.material.color = initialColor;
    }
}
