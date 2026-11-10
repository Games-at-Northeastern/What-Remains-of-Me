using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Boss_CannonTarget : MonoBehaviour
{
    [SerializeField]
    private int hitsToDefeat = 5;

    [SerializeField]
    [Tooltip("Flash visuals for sprites.")]
    private Transform visualRoot;

    [SerializeField]
    private Color hitFlashColor = new Color(1f, 0.45f, 0.45f);

    [SerializeField]
    private float flashTime = 0.15f;

    [Tooltip("Called on every hit with the shot's virus ratio (0 = clean, 1 = virus).")]
    public UnityEvent<float> OnHit;

    [Tooltip("Called when defeated mostly with clean energy.")]
    public UnityEvent OnDefeatedClean;

    [Tooltip("Called when defeated mostly with virus energy.")]
    public UnityEvent OnDefeatedVirus;

    private int hits;
    private float virusTotal;
    private bool defeated;
    private SpriteRenderer[] renderers;
    private Color[] baseColors;

    public int Hits => hits;
    public float AverageVirusRatio => hits == 0 ? 0f : virusTotal / hits;

    private void Awake()
    {
        Transform root = visualRoot != null ? visualRoot : transform;
        renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
        baseColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].color;
        }
    }

    public void TakeHit(float virusRatio)
    {
        if (defeated)
        {
            return;
        }

        hits++;
        virusTotal += virusRatio;
        OnHit?.Invoke(virusRatio);
        StopAllCoroutines();
        StartCoroutine(Flash());

        Debug.Log($"{name} hit {hits}/{hitsToDefeat} (virus {virusRatio:P0}).");

        if (hits < hitsToDefeat)
        {
            return;
        }

        defeated = true;
        if (AverageVirusRatio >= 0.5f)
        {
            Debug.Log($"{name} defeated by virus overflow.");
            OnDefeatedVirus?.Invoke();
        }
        else
        {
            Debug.Log($"{name} freed with clean energy.");
            OnDefeatedClean?.Invoke();
        }
    }

    private IEnumerator Flash()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].color = hitFlashColor;
            }
        }

        yield return new WaitForSeconds(flashTime);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].color = baseColors[i];
            }
        }
    }
}
