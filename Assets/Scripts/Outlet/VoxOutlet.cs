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
    private void Update()
    {
        // slider.value = GetVirus() / 100f;

        if (GetVirus() >= 50f && firstStep)
        {
            door2.sprite = openDoorSprite2;
            doorAnimator2.enabled = false;
            doorCollider2.enabled = false;
            turret1.enabled = false;
            turret2.enabled = false;
            firstDoor.CreateEnergy(firstDoor.GetMaxCharge(), 0);
            leftExit.enabled = true;
            BeatenBoss();
        }
    }

    private void BeatenBoss()
    {
        if(exploded == false)
        {
            exploded = true;
            explosionParticles.Play();
        }

        endingDialogue.StartDialogue();
    }
}


