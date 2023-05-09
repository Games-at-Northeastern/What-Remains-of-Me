using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Opens the door of the Door and Button Prefab when the button of the Door and Button prefab is pressed (collided with)
/// </summary>

public class OpenDoor : MonoBehaviour
{

    //SpriteRenderer of the door
    [SerializeField] private SpriteRenderer doorSpriteRenderer;

    //Collider of the door
    [SerializeField] private Collider2D doorCollider;

    //Sprite for when the door is open
    [SerializeField] private Sprite openDoorSprite;

    //Sprite for when the door is closed
    [SerializeField] private Sprite closedDoorSprite;


    //If the Player or a gameobject with the tag CanPressButton enters the collider of the button,
    //the door collider is disabled and the door sprite is changed to show that the door is open
    //This could be changed to play an animation of the door opening but this works for now
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanPressButton"))
        {
            doorCollider.enabled = false;
            doorSpriteRenderer.sprite = openDoorSprite;
        }

    }

    //While the Player or a gameobject with the tag CanPressButton is within the collider of the button,
    //the door collider stays disabled and the door sprite stays as the open door sprite to show that the door is open
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanPressButton"))
        {
            doorCollider.enabled = false;
            doorSpriteRenderer.sprite = openDoorSprite;
        }
    }


    //When the Player or gameobject with the tag CanPressButton leaves the collider of the button,
    //the door collider is enabled and the door sprite is changed to show that the door is closed
    //This could be changed to play an animation of the door closing but this works for now
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanPressButton"))
        {
            doorCollider.enabled = true;
            doorSpriteRenderer.sprite = closedDoorSprite;
        }

    }


}
