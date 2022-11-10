using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    [SerializeField] private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Sprite closedDoorSprite;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanPressButton"))
        {
            doorCollider.enabled = false;
            doorSpriteRenderer.sprite = openDoorSprite;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanPressButton"))
        {
            doorCollider.enabled = false;
            doorSpriteRenderer.sprite = openDoorSprite;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanPressButton"))
        {
            doorCollider.enabled = true;
            doorSpriteRenderer.sprite = closedDoorSprite;
        }

    }


}
