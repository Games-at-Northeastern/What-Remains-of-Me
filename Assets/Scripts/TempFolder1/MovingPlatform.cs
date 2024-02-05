using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
public class MovingPlatform : MovingElement
{
    protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;

    private Vector2 velocity = new Vector2(0 ,0);

    PlayerController2D player;
    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetComponent<PlayerController2D>() != null)
        {
            if(Vector2.Dot(transform.up, col.GetContact(0).normal) < -0.5f)
            {
                player = col.collider.GetComponent<PlayerController2D>();
            }
        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.GetComponent<PlayerController2D>() == player)
        {
            player = null;
        }
    }

    public Vector2 GetVelocity() => velocity;

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = MovePlatform();
    }
}
