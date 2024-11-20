using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JonesOutlet : AControllable
{

    [SerializeField] private SpriteRenderer door2;
    [SerializeField] private Sprite openDoorSprite2;
    [SerializeField] private Collider2D doorCollider2;
    [SerializeField] private Animator doorAnimator2;

    [SerializeField] private Slider slider;

    [SerializeField] private ParticleSystem explosionParticles;
    private void Update()
    {
        // slider.value = GetVirus() / 100f;

        if (GetVirus() >= 90f)
        {
            door2.sprite = openDoorSprite2;
            doorAnimator2.enabled = false;
            doorCollider2.enabled = false;
        }
    }
}
