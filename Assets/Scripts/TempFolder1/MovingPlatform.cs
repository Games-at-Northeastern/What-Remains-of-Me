using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControllerRefresh;
public class MovingPlatform : MovingElement
{
    protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;

    PlayerController player;
    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetComponent<PlayerController>() != null)
        {
            player = col.collider.GetComponent<PlayerController>();
        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.GetComponent<PlayerController>() != null)
        {
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
    }
}
