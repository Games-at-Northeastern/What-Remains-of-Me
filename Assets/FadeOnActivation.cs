using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class FadeOnActivation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Light2D targetLight;

    [SerializeField] private float duration = 2f;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (targetLight == null)
        {
            targetLight = GetComponent<Light2D>();
        }
    }

    private void OnEnable()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(FadeInSprite());
        }

        if (targetLight != null)
        {
            Debug.Log("Fading in light");
            StartCoroutine(FadeInLight());
        }
    }

    private IEnumerator FadeInSprite()
    {
        float targetAlpha = 1.0f;

        float elapsedTime = 0.0f;
        float startAlpha = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            Color newColor = spriteRenderer.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            spriteRenderer.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = targetAlpha;
        spriteRenderer.color = finalColor;
    }

    private IEnumerator FadeInLight()
    {
        float targetIntensity = 3.0f;

        float elapsedTime = 0.0f;
        float startIntensity = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            targetLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetLight.intensity = targetIntensity;
    }

}
