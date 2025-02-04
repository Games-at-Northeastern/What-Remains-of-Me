using UnityEngine;
using PlayerController;
public class MovingPlatform : MovingElement
{
    protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;

    private PlayerController2D player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController2D>() != null)
        {
            if (transform.position.y < collision.ClosestPoint(transform.position).y)
            {
                player = collision.GetComponent<PlayerController2D>();
                player.OnMovingPlatform = true;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.GetComponent<PlayerController2D>() == player)
        {
            if (rb.velocity.y >= 0)
            {
                player.InternalVelocity += rb.velocity;
            }
            player.OnMovingPlatform = false;
            player = null;
        }
    }

    // Update is called once per frame
    private new void FixedUpdate()
    {
        if (player != null && player.OnMovingPlatform)
        {
            player.ExternalVelocity = MovePlatform();
            player.CombineCurrentVelocities();
        }
        else
        {
            MovePlatform();
        }
    }
}
