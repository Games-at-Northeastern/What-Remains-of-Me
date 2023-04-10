using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// works with Door_and_Outlet prefab to allow Atlas to open the door when enough energy is given
/// </summary>
public class OutletDoor : AControllable
{
    // SpriteRenderer of the door
    [SerializeField] private SpriteRenderer _doorSpriteRenderer;

    // Collider of the door
    [SerializeField] private Collider2D _doorCollider;

    // Sprite for when the door is open
    [SerializeField] private Sprite _openDoorSprite;

    // Sprite for when the door is closed
    [SerializeField] private Sprite _closedDoorSprite;

    /// <summary>
    /// initializes the door closed
    /// </summary>
    private void Awake() => _doorSpriteRenderer.sprite = _closedDoorSprite;

    /// <summary>
    /// if the door has received max energy it opens, otherwise its closed
    /// </summary>
    private void Update()
    {
        if (energy >= maxCharge)
        {
            _doorCollider.enabled = false;
            _doorSpriteRenderer.sprite = _openDoorSprite;
        }
        else
        {
            _doorCollider.enabled = true;
            _doorSpriteRenderer.sprite = _closedDoorSprite;
        }
    }
}
