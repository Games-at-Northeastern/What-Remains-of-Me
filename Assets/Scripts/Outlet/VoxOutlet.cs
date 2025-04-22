using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private ParticleSystem explosionParticles;
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
        cutsceneTrigger.SetActive(true);

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
            explosionParticles.Play();
        }

        //Starting playing his last set of dialogue
        endingDialogue.StartDialogue();

        //Start the sequence for disabling his screen
        StartCoroutine(VoxDisableScreen());
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


