using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JonesOutlet : AControllable
{
    [SerializeField] private SpriteRenderer door;
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Collider2D doorCollider;

    [SerializeField] private Slider slider;

    [SerializeField] private ParticleSystem explosionParticles;
    private void Update()
    {
        slider.value = GetVirus() / 100f;

        if (GetVirus() >= 80f)
        {
            explosionParticles.Play();
            door.sprite = openDoorSprite;
            doorCollider.enabled = false;
        }
    }
}
