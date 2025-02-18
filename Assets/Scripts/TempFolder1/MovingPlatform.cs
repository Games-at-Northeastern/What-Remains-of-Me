using UnityEngine;
using PlayerController;
public class MovingPlatform : MovingElement
{
    protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;

    private PlayerController2D player;

    private const float MaxUpwardVelocityTransfer = 3f;

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
            //Hello future dev. This sections is done to create caps on movement(too high of a value leads to the player flinging up, negatives make jumps very irritating for the player.)
            //Change these values to alter the caps of what parts of velocity are transfered to the player.
            if (rb.linearVelocity.y >= 0)
            {
                Vector2 revisedVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, MaxUpwardVelocityTransfer));
                player.InternalVelocity += revisedVelocity;
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
