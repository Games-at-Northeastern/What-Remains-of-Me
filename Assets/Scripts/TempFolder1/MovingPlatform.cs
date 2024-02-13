using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
public class MovingPlatform : MovingElement
{
    protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;

    PlayerController2D player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController2D>() != null)
        {
            if (transform.position.y < collision.ClosestPoint(transform.position).y)
            {
                Debug.Log("ENTER");
                player = collision.GetComponent<PlayerController2D>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController2D>() == player)
        {
            Debug.Log("EXIT");
            player = null;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            player.ExternalVelocity = MovePlatform();
        }
        else
        {
            MovePlatform();
        }
    }
}
