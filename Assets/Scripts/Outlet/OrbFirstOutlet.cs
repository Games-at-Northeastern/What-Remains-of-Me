using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OrbFirstOutlet : AControllable
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite sprite;
    [SerializeField] private BossRotation br;
    [SerializeField] private Animator[] coverables;
    [SerializeField] private Animator[] unCoverables;
    [SerializeField] private InkDialogueTrigger dialogueTrigger;
    [SerializeField] private NPCOutlet npcOutlet;
    [SerializeField] private Outlet outlet;
    [SerializeField] private Light2D light;
    [SerializeField] private Light2D[] flickerLights;
    private bool activated = false;
    [SerializeField] private float total;

    [SerializeField] private OrbServerAmbientDialogue ambientDialogue;

    [SerializeField] private WireThrower wireThrower;

    private InkTextSwapper textSwapper;

    void Start() {
        textSwapper = GetComponent<InkTextSwapper>();
    }

    private void Update()
    {
        // slider.value = GetVirus() / 100f;
        total = GetEnergy() + GetVirus();
        if (activated == false)
        {
            OrbServerActivate();
        }
        else
        {
            float currentLight = light.intensity;
            float targetLight = 2f;
            light.intensity = Mathf.Lerp(currentLight, targetLight, 0.5f * Time.deltaTime);
        }

    }

    private void OrbServerActivate()
    {
        if (total >= 45f)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
            for (int i = 0; i < unCoverables.Length; i++)
            {
                unCoverables[i].SetBool("Shielded", false);
                unCoverables[i].SetBool("Activate", true);
                unCoverables[i].GetComponentInParent<BoxCollider2D>().enabled = true;
            }
            for (int i = 0; i < coverables.Length; i++)
            {
                coverables[i].SetBool("Shielded", true);
                coverables[i].SetBool("Activate", true);
                coverables[i].GetComponentInParent<BoxCollider2D>().enabled = false;
            }
            for (int i = 0; i < flickerLights.Length; i++)
            {
                StartCoroutine(FlickeringLight(i));
            }
            sr.sprite = sprite;
            br.enabled = true;
            dialogueTrigger.enabled = true;
            npcOutlet.enabled = true;
            outlet.enabled = true;
            activated = true;
            textSwapper.SwapText();
            wireThrower.DisconnectWire();
        }
    }

    IEnumerator FlickeringLight(int i)
    {
        float timeDelay = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = true;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = false;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = true;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = false;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = true;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = false;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = true;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = false;
        timeDelay = Random.Range(0.01f, 0.1f);
        yield return new WaitForSeconds(timeDelay);
        flickerLights[i].enabled = true;
    }
}
