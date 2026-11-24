using UnityEngine;

/// <summary>
///     A laser bullet that flies in a straight line, made at runtime by Boss_LaserType (no prefab needed).
///     It is drawn as a short, thick piece of laser with the same material as the laser beams.
///     Walls, floors, and crates stop the bullet, so they can be used as cover.
///     When it hits the player, the shooter's Boss_AmmunitionType decides what happens.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Boss_LaserBullet : MonoBehaviour
{
    private const int PlayerHierarchyLayerIndex = 3;
    private const float MaxLifetime = 15f;

    private Vector2 direction;
    private float speed;
    private float distanceLeft;
    private float length;
    private float radius;
    private Boss_AmmunitionType ammunition;
    private LineRenderer renderer; // The bullet itself
    private bool finished;

    private ContactFilter2D solidFilter;
    private ContactFilter2D overlapFilter;
    private readonly RaycastHit2D[] rayHits = new RaycastHit2D[8];
    private readonly Collider2D[] overlapHits = new Collider2D[16];
    private readonly string tag = "Player";

    /// <summary>
    ///     Creates a bullet.
    /// </summary>
    /// <param name="position"> Where the bullet starts </param>
    /// <param name="direction"> Which way the bullet flies </param>
    /// <param name="speed"> How fast the bullet moves </param>
    /// <param name="range"> How far the bullet flies before it disappears </param>
    /// <param name="length"> How long the bullet looks, in world units </param>
    /// <param name="width"> How thick the bullet looks, in world units </param>
    /// <param name="material"> Laser material (same as the beams) </param>
    /// <param name="color"> Color of the bullet </param>
    /// <param name="solidLayers"> What stops the bullet </param>
    /// <param name="ammunition"> What happens when the bullet hits the player </param>
    /// <param name="sortingLayerName"> Sorting layer of the bullet </param>
    /// <param name="sortingOrder"> Sorting order of the bullet </param>
    public static Boss_LaserBullet Spawn(Vector2 position, Vector2 direction, float speed, float range, float length,
        float width, Material material, Color color, LayerMask solidLayers, Boss_AmmunitionType ammunition,
        string sortingLayerName, int sortingOrder)
    {
        GameObject bulletObject = new GameObject("Laser Bullet");
        bulletObject.transform.position = position;

        LineRenderer line = bulletObject.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.sharedMaterial = material;
        line.startColor = color;
        line.endColor = color;
        line.startWidth = width;
        line.endWidth = width;
        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        Boss_LaserBullet bullet = bulletObject.AddComponent<Boss_LaserBullet>();
        bullet.renderer = line;
        bullet.direction = direction.normalized;
        bullet.speed = speed;
        bullet.distanceLeft = range;
        bullet.length = length;
        bullet.radius = width * 0.5f;
        bullet.ammunition = ammunition;

        // Walls and crates only, no triggers
        bullet.solidFilter = new ContactFilter2D();
        bullet.solidFilter.SetLayerMask(solidLayers);
        bullet.solidFilter.useTriggers = false;

        // Everything except triggers, so it can find the player
        bullet.overlapFilter = new ContactFilter2D();
        bullet.overlapFilter.NoFilter();
        bullet.overlapFilter.useTriggers = false;

        bullet.DrawBullet(position);
        Destroy(bulletObject, MaxLifetime);
        return bullet;
    }

    /// <summary>
    ///     Moves the bullet, and checks if it hit a wall or the player.
    /// </summary>
    private void Update()
    {
        if (finished)
        {
            return;
        }

        float step = Mathf.Min(speed * Time.deltaTime, distanceLeft);
        Vector2 position = transform.position;

        if (HitsWall(position, step, out Vector2 wallPoint))
        {
            transform.position = wallPoint;
            CheckPlayerHit(); // The player could be right at the wall
            Finish();
            return;
        }

        position += direction * step;
        transform.position = position;
        distanceLeft -= step;
        DrawBullet(position);

        CheckPlayerHit();

        if (!finished && distanceLeft <= 0f)
        {
            Finish(); // Out of range
        }
    }

    /// <summary>
    ///     Draws the bullet as a short laser behind its front point.
    /// </summary>
    /// <param name="front"> The front of the bullet </param>
    private void DrawBullet(Vector2 front)
    {
        renderer.SetPosition(0, front - direction * length);
        renderer.SetPosition(1, front);
    }

    /// <summary>
    ///     Checks if there is a wall in front of the bullet this frame.
    /// </summary>
    /// <param name="position"> Current position </param>
    /// <param name="step"> How far the bullet moves this frame </param>
    /// <param name="wallPoint"> Where the bullet hits the wall </param>
    /// <returns> Returns whether the bullet hit a wall </returns>
    private bool HitsWall(Vector2 position, float step, out Vector2 wallPoint)
    {
        wallPoint = position;
        int count = Physics2D.Raycast(position, direction, solidFilter, rayHits, step + radius);
        float closestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D other = rayHits[i].collider;
            if (other == null || IsPlayer(other.transform, out _))
            {
                continue; // The player never blocks a bullet
            }

            if (rayHits[i].distance < closestDistance)
            {
                closestDistance = rayHits[i].distance;
                wallPoint = rayHits[i].point;
            }
        }

        return closestDistance < float.MaxValue;
    }

    /// <summary>
    ///     Checks if the bullet is touching the player.
    /// </summary>
    private void CheckPlayerHit()
    {
        int count = Physics2D.OverlapCircle(transform.position, radius, overlapFilter, overlapHits);
        for (int i = 0; i < count; i++)
        {
            if (overlapHits[i] != null && IsPlayer(overlapHits[i].transform, out Transform player))
            {
                if (ammunition != null)
                {
                    ammunition.HitOnce(player);
                }
                Finish();
                return;
            }
        }
    }

    private bool IsPlayer(Transform item, out Transform found) =>
        UtilityFunctions.CompareTagOfHierarchy(item, tag, out found, PlayerHierarchyLayerIndex);

    private void Finish()
    {
        finished = true;
        Destroy(gameObject);
    }
}
