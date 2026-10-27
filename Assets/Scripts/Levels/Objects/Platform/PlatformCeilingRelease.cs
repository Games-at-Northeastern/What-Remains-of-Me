using PlayerController;
using UnityEngine;

/// <summary>
/// Lets the player drop through a rising platform when it would carry them into a ceiling,
/// instead of being squeezed into the surrounding walls.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformCeilingRelease : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private float restoreDistance = 0.05f;

    private Rigidbody2D rb;
    private PlayerController2D player;
    private Collider2D playerCollider;
    private bool isReleased;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player == null && collision.TryGetComponent(out PlayerController2D controller))
        {
            player = controller;
            playerCollider = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isReleased && collision == playerCollider)
        {
            player = null;
            playerCollider = null;
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (isReleased)
        {
            if (platformCollider.Distance(playerCollider).distance > restoreDistance)
            {
                Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
                isReleased = false;
                player = null;
                playerCollider = null;
            }
            return;
        }

        bool isAbovePlatform = playerCollider.bounds.min.y >= platformCollider.bounds.max.y - 0.1f;
        if (rb.linearVelocity.y > 0 && isAbovePlatform && player.TouchingCeiling())
        {
            Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
            player.OnMovingPlatform = false;
            player.RemoveForce(gameObject);
            isReleased = true;
        }
    }
}
