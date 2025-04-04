using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class VoxOutlet : AControllable
{

    [SerializeField] private SpriteRenderer door2;
    [SerializeField] private Sprite openDoorSprite2;
    [SerializeField] private Collider2D doorCollider2;
    [SerializeField] private Animator doorAnimator2;
    [SerializeField] private Slider slider;
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

        if (GetVirus() >= 50f && firstStep)
        {
            BeatenBoss();
        }
    }

    public void BeatenBoss()
    {
        cutsceneTrigger.SetActive(true);

        door2.sprite = openDoorSprite2;
        doorAnimator2.enabled = false;
        doorCollider2.enabled = false;
        turret1.enabled = false;
        turret2.enabled = false;
        firstDoor.CreateEnergy(firstDoor.GetMaxCharge(), 0);
        leftExit.enabled = true;

        if (exploded == false)
        {
            exploded = true;
            explosionParticles.Play();
        }

        endingDialogue.StartDialogue();

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


