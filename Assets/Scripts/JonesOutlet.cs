using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JonesOutlet : AControllable
{
    [SerializeField] private SpriteRenderer door;
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Collider2D doorCollider;

    [SerializeField] private ParticleSystem explosionParticles;
    private void Update()
    {
        if (GetVirus() >= 80f)
        {
            explosionParticles.Play();
            door.sprite = openDoorSprite;
            doorCollider.enabled = false;
        }
    }
}
