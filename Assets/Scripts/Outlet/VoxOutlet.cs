using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Outlet that acts like the main healthbar of vox.
/// </summary>
public class VoxOutlet : AControllable
{

    [SerializeField] private SpriteRenderer door2;
    [SerializeField] private Sprite openDoorSprite2;
    [SerializeField] private Collider2D doorCollider2;
    [SerializeField] private Animator doorAnimator2;
    [SerializeField] private VirusTurret turret1;
    [SerializeField] private VirusTurret turret2;
    [SerializeField] private ControllableDoor firstDoor;
    [SerializeField] private ElevatorController leftExit;
    [SerializeField] public bool firstStep = false;

    [Header("Death Sequence")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem debrisParticles;
    [SerializeField] private float explosionDuration = 2f;
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private bool exploded;
    [SerializeField] private InkDialogueTrigger endingDialogue;

    [SerializeField] private float disableScreenTime = 8f;
    [SerializeField] private GameObject voxDefeatedScreen;
    [SerializeField] private List<GameObject> voxDefeatedDisable;

    [SerializeField] private GameObject cutsceneTrigger;
    private void Update()
    {
        // slider.value = GetVirus() / 100f;
        //Once over half virus, end the fight
        if (GetVirus() >= 50f && firstStep)
        {
            BeatenBoss();
        }
    }

    public void BeatenBoss()
    {
        //Start the ending cutscene
        if (cutsceneTrigger)
        {
            cutsceneTrigger.SetActive(true);
        }

        //Open the right door
        door2.sprite = openDoorSprite2;
        doorAnimator2.enabled = false;
        doorCollider2.enabled = false;

        //Turn off the turrets
        turret1.enabled = false;
        turret2.enabled = false;

        //Power up the left door to let Altas exit
        firstDoor.CreateEnergy(firstDoor.GetMaxCharge(), 0);
        leftExit.enabled = true;


        //Explosions, fire, death, destruction
        if (exploded == false)
        {
            exploded = true;
            StartCoroutine(PlayBossDeathExplosion());
        }
        
        //Starting playing his last set of dialogue
        endingDialogue.StartDialogue();

        //Start the sequence for disabling his screen
        StartCoroutine(VoxDisableScreen());
    }

    /// <summary>
    /// Play the Vox death explosion sequence.
    /// </summary> 
    private IEnumerator PlayBossDeathExplosion()
    {
        var t = 0f;
        var nextExplosion = 0f;
        
        while (t < explosionDuration)
        {
            var p = t / explosionDuration;
            if (t >= nextExplosion)
            {
                // Calculate random offset for explosion particle
                var offset = Random.insideUnitCircle * Mathf.Lerp(explosionRadius * 0.35f, explosionRadius, p);
                var pos = explosionParticles.transform.position + new Vector3(offset.x, offset.y, 0f);
                
                // Instantiate the explosion and interpolate size/speed
                var explosion = Instantiate(explosionParticles, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
                var main = explosion.main;
                main.startSizeMultiplier *= Mathf.Lerp(0.8f, 1.8f, p);
                main.startSpeedMultiplier *= Mathf.Lerp(0.8f, 1.4f, p);
                
                // Play once and destroy
                explosion.Play();
                Destroy(explosion.gameObject, main.duration + main.startLifetime.constantMax + 0.5f);
                
                nextExplosion = t + Mathf.Lerp(0.18f, 0.05f, p);
            }

            yield return null;
            t += Time.deltaTime;
        }

        // Play debris scatter particles
        for (var i = 0; i < 8; i++)
        {
            // Calculate random offset
            var offset = Random.insideUnitCircle * 2.8f;
            var pos = explosionParticles.transform.position + new Vector3(offset.x, offset.y, 0f);
            
            // Instantiate and randomize size/speed
            var debris = Instantiate(debrisParticles, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            var main = debris.main;
            main.startSizeMultiplier *= Random.Range(0.85f, 1.35f);
            main.startSpeedMultiplier *= Random.Range(0.9f, 1.4f);
            
            // Play once and destroy
            debris.Play();
            Destroy(debris.gameObject, main.duration + main.startLifetime.constantMax + 0.5f);
        }
    }

    private IEnumerator VoxDisableScreen() {
        yield return new WaitForSeconds(disableScreenTime);

        // Make screen true
        voxDefeatedScreen.SetActive(true);

        // Disable lights and wires from Vox
        foreach (GameObject g in voxDefeatedDisable) {
            g.SetActive(false);
        }
    }
}


