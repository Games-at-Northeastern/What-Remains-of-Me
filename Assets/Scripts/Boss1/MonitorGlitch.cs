using System.Collections;
using UnityEngine;

public class MonitorGlitch : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite voxSprite;

    [Header("Fade Settings")]
    public float fadeDuration = 3f;

    private SpriteRenderer sr;
    private SpriteRenderer overlaySr;
    private Vector3 normalScale;
    private Vector3 voxScale;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        normalScale = transform.localScale;

        if (normalSprite != null && voxSprite != null)
        {
            Vector2 normalSize = normalSprite.bounds.size;
            Vector2 voxSize = voxSprite.bounds.size;
            voxScale = new Vector3(
                normalScale.x * (normalSize.x / voxSize.x),
                normalScale.y * (normalSize.y / voxSize.y),
                1f
            );
        }

        // Create a child object with its own SpriteRenderer for the overlay
        GameObject overlayObj = new GameObject("Overlay");
        overlayObj.transform.SetParent(transform, false);
        overlaySr = overlayObj.AddComponent<SpriteRenderer>();
        overlaySr.sortingLayerID = sr.sortingLayerID;
        overlaySr.sortingOrder = sr.sortingOrder + 1;
        overlaySr.color = new Color(1f, 1f, 1f, 0f);

        sr.sprite = normalSprite;
    }

    public void StartGlitching()
    {
        StopAllCoroutines();
        StartCoroutine(CrossfadeTo(voxSprite, voxScale));
    }

    public void StopGlitching()
    {
        StopAllCoroutines();
        StartCoroutine(CrossfadeTo(normalSprite, normalScale));
    }

    private IEnumerator CrossfadeTo(Sprite target, Vector3 targetScale)
    {
        // Set the overlay to the target sprite, fully transparent
        overlaySr.sprite = target;
        overlaySr.transform.localScale = new Vector3( targetScale.x / transform.localScale.x, targetScale.y / transform.localScale.y, 1f);
        overlaySr.color = new Color(1f, 1f, 1f, 0f);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            overlaySr.color = new Color(1f, 1f, 1f, t);
            yield return null;
        }

        // Swap base sprite and clear overlay
        sr.sprite = target;
        transform.localScale = targetScale;
        overlaySr.color = new Color(1f, 1f, 1f, 0f);
        overlaySr.sprite = null;
    }
}
