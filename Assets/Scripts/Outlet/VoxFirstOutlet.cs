using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for the bottom outlet of 1.5.2, whereas the player needs to fill it with eneough energy to open up the bottom level and start feeding virus to Vox
/// </summary>
public class VoxFirstOutlet : AControllable
{
    [SerializeField][Range(0, 100)] private float virusNeeded = 50f;
    [SerializeField] private SpriteRenderer door2;
    [SerializeField] private Sprite openDoorSprite2;
    [SerializeField] private Collider2D doorCollider2;
    [SerializeField] private Animator doorAnimator2;
    [SerializeField] private MovingElementController elevatorSolve;
    [SerializeField] private WireThrower wire;
    [SerializeField] private VoxOutlet secondStepOutlet;
    [SerializeField] private Animator secondOutletAnimator;
    [SerializeField] private ParticleSystem explosionParticles;

    private bool hasTriggered = false; // Ensure coroutine is only started once
    private void Update()
    {
        // slider.value = GetVirus() / 100f;

        //Debug.Log("Current Virus Level: " + GetVirus());

        //if (GetVirus() >= 30f)
        //{
        //    door2.sprite = openDoorSprite2;
        //    doorAnimator2.enabled = false;
        //    doorCollider2.enabled = false;
        //    elevatorSolve.CreateEnergy(20, 0);
        //    GetComponent<SpriteRenderer>().color = Color.gray;
        //    GetComponent<BoxCollider2D>().enabled = false;
        //    wire.Invoke("Disconnect", 0.0f);
        //    GetComponent<VoxFirstOutlet>().enabled = false;
        //    secondStepOutlet.firstStep = true;

        //}

        if (GetVirus() >= virusNeeded && hasTriggered == false)
        {
            StartCoroutine(OpenDoor());
        }
    }

    /// <summary>
    /// The sequence to occur once the second outlet is opened
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenDoor()
    {
        //Open up the second door(the right one), of which it'll single the left door to open
        doorAnimator2.SetBool("Opening", true);
        //Give a delay
        yield return new WaitForSecondsRealtime(2);
        hasTriggered = true;
        //Door has opened, start moving down
        door2.sprite = openDoorSprite2;
        doorAnimator2.enabled = false;
        doorCollider2.enabled = false;
        //Make sure the elevator starts working
        elevatorSolve.CreateEnergy(20, 0);

        //Disable this outlet
        GetComponent<SpriteRenderer>().color = Color.gray;
        GetComponent<BoxCollider2D>().enabled = false;
        wire.Invoke("Disconnect", 0.0f);
        GetComponent<VoxFirstOutlet>().enabled = false;

        //Enable the actual vox outlet(go to that script for contiuation)
        secondStepOutlet.firstStep = true;
        secondOutletAnimator.SetBool("Shielded", false);
        secondOutletAnimator.SetBool("Activate", true);
        secondOutletAnimator.GetComponentInParent<BoxCollider2D>().enabled = true;
        

        yield break;
    }

}
