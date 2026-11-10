using System;
using UnityEngine;

public class Boss_CannonProjectile : MonoBehaviour
{
    private const int PlayerHierarchyLayerIndex = 3;
    private const string PlayerTag = "Player";
    private const float MaxLifetime = 15f;

    [Serializable]
    public class Settings
    {
        [Tooltip("Units per second.")]
        public float speed = 6f;

        [Tooltip("Distance travel before disappearing.")]
        public float range = 8f;

        [Tooltip("Visual && Hit size.")]
        public float radius = 0.3f;

        [Tooltip("Sprite for projectile.")]
        public Sprite sprite;

        public Color color = Color.white;

        [Tooltip("Layer for projectile.")]
        public string sortingLayerName = "Foreground";

        public int sortingOrder = 50;

        [Tooltip("Covers for projectile.")]
        public LayerMask solidLayers = 1; // Default
    }

    private Settings settings;
    private Vector2 direction;
    private float distanceLeft;
    private bool hurtsPlayer;
    private bool hitsTargets;
    private float virusRatio;
    private SpikeTeleport deathTeleporter;
    private ContactFilter2D solidFilter;
    private ContactFilter2D overlapFilter;
    private readonly RaycastHit2D[] rayHits = new RaycastHit2D[8];
    private readonly Collider2D[] overlapHits = new Collider2D[16];
    private bool finished;

    public static Boss_CannonProjectile Spawn(Vector2 position, Vector2 direction, Settings settings,
        bool hurtsPlayer, bool hitsTargets, float virusRatio = 0f,
        SpikeTeleport deathTeleporter = null, Color? colorOverride = null)
    {
        var go = new GameObject(hurtsPlayer ? "Enemy Projectile" : "Cannon Projectile");
        go.transform.position = position;

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = settings.sprite;
        spriteRenderer.color = colorOverride ?? settings.color;
        spriteRenderer.sortingLayerName = settings.sortingLayerName;
        spriteRenderer.sortingOrder = settings.sortingOrder;

        if (settings.sprite != null)
        {
            float spriteSize = Mathf.Max(settings.sprite.bounds.size.x, settings.sprite.bounds.size.y);
            if (spriteSize > 0f)
            {
                go.transform.localScale = Vector3.one * (settings.radius * 2f / spriteSize);
            }
        }

        var projectile = go.AddComponent<Boss_CannonProjectile>();
        projectile.settings = settings;
        projectile.direction = direction.normalized;
        projectile.distanceLeft = settings.range;
        projectile.hurtsPlayer = hurtsPlayer;
        projectile.hitsTargets = hitsTargets;
        projectile.virusRatio = virusRatio;
        projectile.deathTeleporter = deathTeleporter;

        projectile.solidFilter = new ContactFilter2D();
        projectile.solidFilter.SetLayerMask(settings.solidLayers);
        projectile.solidFilter.useTriggers = false;

        projectile.overlapFilter = new ContactFilter2D();
        projectile.overlapFilter.NoFilter(); // includes triggers, so it can find hitboxes

        Destroy(go, MaxLifetime);
        return projectile;
    }

    private void Update()
    {
        if (finished)
        {
            return;
        }

        float step = Mathf.Min(settings.speed * Time.deltaTime, distanceLeft);
        Vector2 position = transform.position;

        // 1. Did we run into something solid this frame?
        if (TryGetSolidHit(position, step, out RaycastHit2D wallHit))
        {
            transform.position = wallHit.point;
            CheckOverlaps(); 
            Finish();
            return;
        }

        // 2. Move
        position += direction * step;
        transform.position = position;
        distanceLeft -= step;

        // 3. Did we touch Atlas or a target?
        CheckOverlaps();

        if (!finished && distanceLeft <= 0f)
        {
            Finish();
        }
    }

    private bool TryGetSolidHit(Vector2 position, float step, out RaycastHit2D closest)
    {
        closest = default;
        int count = Physics2D.Raycast(position, direction, solidFilter, rayHits, step + settings.radius);
        float best = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D hitCollider = rayHits[i].collider;
            if (hitCollider == null || IsPlayer(hitCollider.transform, out _))
            {
                continue; // Atlas never blocks a shot
            }

            if (rayHits[i].distance < best)
            {
                best = rayHits[i].distance;
                closest = rayHits[i];
            }
        }

        return best < float.MaxValue;
    }

    private void CheckOverlaps()
    {
        int count = Physics2D.OverlapCircle(transform.position, settings.radius, overlapFilter, overlapHits);

        for (int i = 0; i < count && !finished; i++)
        {
            Collider2D other = overlapHits[i];
            if (other == null)
            {
                continue;
            }

            if (hurtsPlayer && IsPlayer(other.transform, out Transform player))
            {
                KillPlayer(player);
                Finish();
                return;
            }

            if (hitsTargets)
            {
                Boss_CannonTarget target = other.GetComponentInParent<Boss_CannonTarget>();
                if (target != null)
                {
                    target.TakeHit(virusRatio);
                    Finish();
                    return;
                }
            }
        }
    }

    private void KillPlayer(Transform player)
    {
        if (PlayerRef.PlayerManager != null && PlayerRef.PlayerManager.GodMode)
        {
            return;
        }

        if (deathTeleporter == null)
        {
            Debug.LogWarning("Boss_CannonProjectile hit Atlas but has no death teleporter assigned.");
            return;
        }

        deathTeleporter.StartCoroutine(deathTeleporter.PerformDeath(player.gameObject));

        if (SoundController.instance != null)
        {
            SoundController.instance.PlaySound("Laser_Death_Sound");
        }
    }

    private static bool IsPlayer(Transform t, out Transform player) =>
        UtilityFunctions.CompareTagOfHierarchy(t, PlayerTag, out player, PlayerHierarchyLayerIndex);

    private void Finish()
    {
        finished = true;
        Destroy(gameObject);
    }
}
