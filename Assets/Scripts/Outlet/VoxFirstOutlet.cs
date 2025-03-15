using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoxFirstOutlet : AControllable
{

    [SerializeField] private SpriteRenderer door2;
    [SerializeField] private Sprite openDoorSprite2;
    [SerializeField] private Collider2D doorCollider2;
    [SerializeField] private Animator doorAnimator2;
    [SerializeField] private MovingElementController elevatorSolve;
    [SerializeField] private Slider slider;
    [SerializeField] private WireThrower wire;
    [SerializeField] private VoxOutlet secondStepOutlet;

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

        if (GetVirus() >= 30f && hasTriggered == false)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        doorAnimator2.SetBool("Opening", true);

        yield return new WaitForSecondsRealtime(2);
        hasTriggered = true;
        elevatorSolve.CreateEnergy(20, 0);
        GetComponent<SpriteRenderer>().color = Color.gray;
        GetComponent<BoxCollider2D>().enabled = false;
        wire.Invoke("Disconnect", 0.0f);
        GetComponent<VoxFirstOutlet>().enabled = false;
        secondStepOutlet.firstStep = true;
        door2.sprite = openDoorSprite2;
        doorAnimator2.enabled = false;
        doorCollider2.enabled = false;

        yield break;
    }

}
