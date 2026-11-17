using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Laser module: decides how the laser shoots.
///     There are 4 laser types:
///     1. Hitscan: a steady beam (like the death laser). Can turn on and off on a timer.
///     2. LaserBullet: fast single laser bullets (Star Wars type).
///     3. Sniper: dotted aim line, then beeps while the aim is locked, then shoots along that line.
///     4. Tracking: a steady beam whose end slowly follows the player (Helldivers 2 orbital laser type).
///     Dotted lines always mean "this laser is about to shoot here".
///     Use together with Boss_SentryType (how it moves) and Boss_AmmunitionType (what the shots do).
///     Boss_SentryType is optional. Without it (or with it turned off), the laser acts like a Static sentry.
///     The laser shoots along its -up, the same as the death laser turrets.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Boss_LaserType : MonoBehaviour
{
    private const int PlayerHierarchyLayerIndex = 3;
    private const int MaxDashes = 80;

    [SerializeField]
    [Tooltip("Hitscan: steady beam.\nLaserBullet: fast bullets.\nSniper: aim line, beeps, then one shot.\nTracking: beam that slowly follows the player.")]
    private LaserType laserType;

    [SerializeField]
    [Tooltip("How far in front of the turret the shots start.")]
    private float muzzleDistance = 0.8f;
    [SerializeField]
    [Tooltip("How long the laser waits before it starts working. Use this to offset lasers from each other.")]
    private float initialDelay;
    [SerializeField]
    [Tooltip("What blocks the shots (make sure default is always included!)")]
    private LayerMask solidLayers = 1;
    [SerializeField]
    [Tooltip("How far beams and sniper shots can reach.")]
    private float range = 30f;

    [Header("Dotted Line (aim and warning)")]
    [SerializeField]
    private float dashLength = 0.35f;
    [SerializeField]
    private float gapLength = 0.25f;
    [SerializeField]
    private float dottedLineWidth = 0.07f;

    [Header("Hitscan and Tracking (beams)")]
    [SerializeField]
    [Tooltip("How long the beam stays on. 0 or less = always on.")]
    private float beamOnTime = 2f;
    [SerializeField]
    [Tooltip("How long the beam stays off between shots.")]
    private float beamOffTime = 1.5f;
    [SerializeField]
    [Tooltip("How long the dotted warning line shows before the beam turns on.")]
    private float warningTime = 0.6f;
    [SerializeField]
    private float beamWidth = 0.3f;
    [SerializeField]
    [Tooltip("Tracking only: how fast the end of the beam moves towards the player.")]
    private float trackingSpeed = 2.5f;

    [Header("Laser Bullet")]
    [SerializeField]
    [Tooltip("Time between each burst of bullets.")]
    private float fireInterval = 1f;
    [SerializeField]
    [Tooltip("How many bullets are in one burst.")]
    private int bulletsPerBurst = 1;
    [SerializeField]
    [Tooltip("Time between each bullet in a burst.")]
    private float burstSpacing = 0.12f;
    [SerializeField]
    private float bulletSpeed = 18f;
    [SerializeField]
    [Tooltip("How far a bullet flies before it disappears.")]
    private float bulletRange = 22f;
    [SerializeField]
    [Tooltip("How long a bullet looks, in world units.")]
    private float bulletLength = 1f;
    [SerializeField]
    [Tooltip("How thick a bullet looks, in world units.")]
    private float bulletWidth = 0.28f;
    [SerializeField]
    private string bulletSortingLayer = "Foreground";
    [SerializeField]
    private int bulletSortingOrder = 50;

    [Header("Sniper")]
    [SerializeField]
    [Tooltip("How long the dotted aim line follows the player.")]
    private float aimTime = 1.2f;
    [SerializeField]
    [Tooltip("How long the aim stays locked (and beeps) before the shot. This is when the player can dodge.")]
    private float lockTime = 0.8f;
    [SerializeField]
    [Tooltip("How many beeps play while the aim is locked.")]
    private int beepCount = 2;
    [SerializeField]
    [Tooltip("How long the line flashes on each beep.")]
    private float beepFlashTime = 0.12f;
    [SerializeField]
    [Tooltip("Hitscan: a thick laser shot along the line.\nBullet: a very fast laser bullet along the line.")]
    private SniperShot sniperShot;
    [SerializeField]
    private float shotWidth = 0.4f;
    [SerializeField]
    [Tooltip("How long the hitscan shot is shown on screen.")]
    private float shotFlashTime = 0.15f;
    [SerializeField]
    [Tooltip("Speed of the bullet when Sniper Shot is Bullet.")]
    private float sniperBulletSpeed = 40f;
    [SerializeField]
    [Tooltip("How long the laser waits after a shot.")]
    private float cooldown = 1f;

    [Header("Sound")]
    [SerializeField]
    [Tooltip("Plays when a Hitscan or Tracking beam turns on.")]
    private AudioClip beamClip;
    [SerializeField]
    [Tooltip("Plays on each laser bullet.")]
    private AudioClip bulletClip;
    [SerializeField]
    [Tooltip("Plays on each sniper beep while the aim is locked.")]
    private AudioClip beepClip;
    [SerializeField]
    [Tooltip("Plays when the sniper shoots.")]
    private AudioClip sniperShotClip;
    [SerializeField]
    [Range(0f, 1f)]
    private float volume = 0.6f;

    private LineRenderer renderer; // Beams and the sniper shot
    private readonly List<LineRenderer> dashes = new List<LineRenderer>(); // Dotted line pieces
    private Boss_SentryType sentry;
    private Boss_AmmunitionType ammunition;
    private Material defaultMaterial;
    private float delayLeft;

    private Phase phase = Phase.Idle;
    private float phaseTimer;
    private int beepsLeft;
    private float beepFlashLeft;
    private Vector2 trackingPoint; // Where the tracking beam is aimed

    private float fireTimer;
    private int burstLeft; // Bullets left in the current burst
    private float burstTimer;

    private ContactFilter2D shotFilter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[16];
    private readonly string tag = "Player";

    private void Awake()
    {
        renderer = GetComponent<LineRenderer>();
        renderer.useWorldSpace = true;
        renderer.positionCount = 2;
        defaultMaterial = renderer.sharedMaterial;
        HideLine();

        sentry = GetComponent<Boss_SentryType>();
        ammunition = GetComponent<Boss_AmmunitionType>();

        // This module replaces the death laser on this object, so turn the old one off (only on this object)
        MovingDeathLaser oldLaser = GetComponent<MovingDeathLaser>();
        if (oldLaser != null)
        {
            oldLaser.enabled = false;
        }

        // Every layer so beams can find the player, no triggers
        shotFilter = new ContactFilter2D();
        shotFilter.NoFilter();
        shotFilter.useTriggers = false;

        delayLeft = initialDelay;
    }

    private void OnDisable()
    {
        HideLine();
        ResetCycle();
    }

    /// <summary>
    ///     Runs the current laser type.
    /// </summary>
    private void Update()
    {
        if (delayLeft > 0f)
        {
            delayLeft -= Time.deltaTime;
            return;
        }

        // Use the ammunition's look (death or virus)
        Material material = LineMaterial;
        if (renderer.sharedMaterial != material)
        {
            renderer.sharedMaterial = material;
            foreach (LineRenderer dash in dashes)
            {
                dash.sharedMaterial = material;
            }
        }

        switch (laserType)
        {
            case LaserType.Hitscan:
            case LaserType.Tracking:
                UpdateBeam();
                break;
            case LaserType.LaserBullet:
                UpdateLaserBullet();
                break;
            case LaserType.Sniper:
                UpdateSniper();
                break;
        }
    }

    /// <summary>
    ///     Steady beam: dotted warning line, beam on, beam off, repeat.
    ///     Tracking moves the end of the beam towards the player while it is on.
    /// </summary>
    private void UpdateBeam()
    {
        switch (phase)
        {
            case Phase.Idle:
                HideLine();
                if (HasTarget())
                {
                    trackingPoint = Muzzle + FireDirection * 3f; // Tracking starts where the turret is facing
                    if (warningTime > 0f)
                    {
                        phase = Phase.Aiming;
                        phaseTimer = warningTime;
                    }
                    else
                    {
                        StartBeam();
                    }
                }
                break;

            case Phase.Aiming: // Warning
                MoveTrackingPoint();
                DrawDottedLine(CastBeam(BeamDirection, out _), LineColor(0.6f), dottedLineWidth);
                phaseTimer -= Time.deltaTime;
                if (phaseTimer <= 0f)
                {
                    StartBeam();
                }
                break;

            case Phase.Firing: // Beam on
                MoveTrackingPoint();
                Vector2 end = CastBeam(BeamDirection, out Transform hitPlayer);
                DrawLine(end, LineColor(1f), beamWidth);
                if (hitPlayer != null && ammunition != null)
                {
                    ammunition.HitOverTime(hitPlayer, Time.deltaTime);
                }

                // 0 or less means the beam never turns off
                if (beamOnTime > 0f)
                {
                    phaseTimer -= Time.deltaTime;
                    if (phaseTimer <= 0f)
                    {
                        HideLine();
                        phase = Phase.Cooldown;
                        phaseTimer = beamOffTime;
                    }
                }
                break;

            case Phase.Cooldown:
                phaseTimer -= Time.deltaTime;
                if (phaseTimer <= 0f)
                {
                    phase = Phase.Idle;
                }
                break;
        }
    }

    private void StartBeam()
    {
        HideDashes();
        phase = Phase.Firing;
        phaseTimer = beamOnTime;
        PlayClip(beamClip);
    }

    /// <summary>
    ///     Shoots bursts of fast laser bullets.
    /// </summary>
    private void UpdateLaserBullet()
    {
        // Finish the current burst first
        if (burstLeft > 0)
        {
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0f)
            {
                FireBullet(bulletSpeed);
                PlayClip(bulletClip);
                burstLeft--;
                burstTimer = burstSpacing;
            }
            return;
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f || !HasTarget() || (sentry != null && !sentry.IsAimedAtPlayer()))
        {
            return;
        }

        burstLeft = Mathf.Max(1, bulletsPerBurst);
        burstTimer = 0f;
        fireTimer = fireInterval;
    }

    private void FireBullet(float speed)
    {
        Color color = ammunition != null ? ammunition.BulletColor : Color.red;
        Boss_LaserBullet.Spawn(Muzzle, FireDirection, speed, bulletRange, bulletLength, bulletWidth,
            LineMaterial, color, solidLayers, ammunition, bulletSortingLayer, bulletSortingOrder);
    }

    /// <summary>
    ///     Dotted aim line (follows) -> aim locks and beeps -> shot -> wait.
    /// </summary>
    private void UpdateSniper()
    {
        switch (phase)
        {
            case Phase.Idle:
                HideLine();
                if (HasTarget())
                {
                    phase = Phase.Aiming;
                    phaseTimer = aimTime;
                }
                break;

            case Phase.Aiming:
                if (!HasTarget())
                {
                    ResetCycle(); // Lost the player before shooting
                    HideLine();
                    break;
                }

                DrawDottedLine(CastBeam(FireDirection, out _), LineColor(0.6f), dottedLineWidth);
                phaseTimer -= Time.deltaTime;
                if (phaseTimer <= 0f)
                {
                    phase = Phase.Locked;
                    phaseTimer = lockTime;
                    beepsLeft = Mathf.Max(1, beepCount);
                    beepFlashLeft = 0f;
                    SetAimFrozen(true); // The aim stops here, so the player can dodge
                }
                break;

            case Phase.Locked:
                // Beep evenly during the lock time, and flash the line on each beep
                float beepSpacing = lockTime / Mathf.Max(1, beepCount);
                if (beepsLeft > 0 && phaseTimer <= beepsLeft * beepSpacing)
                {
                    beepsLeft--;
                    beepFlashLeft = beepFlashTime;
                    PlayClip(beepClip);
                }

                bool flashing = beepFlashLeft > 0f;
                beepFlashLeft -= Time.deltaTime;
                DrawDottedLine(CastBeam(FireDirection, out _), LineColor(flashing ? 1f : 0.6f), dottedLineWidth * (flashing ? 2.5f : 1f));

                phaseTimer -= Time.deltaTime;
                if (phaseTimer <= 0f)
                {
                    FireSniperShot();
                }
                break;

            case Phase.Firing: // Showing the hitscan shot
                phaseTimer -= Time.deltaTime;
                if (phaseTimer <= 0f)
                {
                    HideLine();
                    SetAimFrozen(false);
                    phase = Phase.Cooldown;
                    phaseTimer = cooldown;
                }
                break;

            case Phase.Cooldown:
                phaseTimer -= Time.deltaTime;
                if (phaseTimer <= 0f)
                {
                    phase = Phase.Idle;
                }
                break;
        }
    }

    /// <summary>
    ///     Shoots along the locked line, as a thick laser or as a very fast bullet.
    /// </summary>
    private void FireSniperShot()
    {
        HideDashes();

        PlayClip(sniperShotClip);

        if (sniperShot == SniperShot.Bullet)
        {
            FireBullet(sniperBulletSpeed);
            phase = Phase.Firing;
            phaseTimer = 0f;
            return;
        }

        Vector2 end = CastBeam(FireDirection, out Transform hitPlayer);
        DrawLine(end, LineColor(1f), shotWidth);
        if (hitPlayer != null && ammunition != null)
        {
            ammunition.HitOnce(hitPlayer);
        }

        phase = Phase.Firing;
        phaseTimer = shotFlashTime;
    }

    /// <summary>
    ///     Tracking only: moves the beam's aim point towards the player.
    /// </summary>
    private void MoveTrackingPoint()
    {
        if (laserType != LaserType.Tracking)
        {
            return;
        }

        Transform player = GetPlayer();
        if (player != null)
        {
            trackingPoint = Vector2.MoveTowards(trackingPoint, player.position, trackingSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    ///     Finds where a beam stops: the player, or the first solid object.
    /// </summary>
    /// <param name="direction"> Which way the beam goes </param>
    /// <param name="hitPlayer"> The player, if the beam hits them </param>
    /// <returns> Returns the end point of the beam </returns>
    private Vector2 CastBeam(Vector2 direction, out Transform hitPlayer)
    {
        hitPlayer = null;
        Vector2 origin = Muzzle;
        int count = Physics2D.Raycast(origin, direction, shotFilter, hits, range);
        Array.Sort(hits, 0, count, HitDistanceComparer.Instance); // Closest hit first

        for (int i = 0; i < count; i++)
        {
            Collider2D other = hits[i].collider;
            if (other == null || other.transform.IsChildOf(transform))
            {
                continue;
            }

            if (UtilityFunctions.CompareTagOfHierarchy(other.transform, tag, out hitPlayer, PlayerHierarchyLayerIndex))
            {
                return hits[i].point;
            }

            if ((solidLayers.value & (1 << other.gameObject.layer)) != 0)
            {
                return hits[i].point;
            }
        }

        return origin + direction * range;
    }

    private bool HasTarget() => sentry == null || sentry.HasTarget;

    private void SetAimFrozen(bool frozen)
    {
        if (sentry != null)
        {
            sentry.AimFrozen = frozen;
        }
    }

    private void ResetCycle()
    {
        phase = Phase.Idle;
        burstLeft = 0;
        SetAimFrozen(false);
    }

    private Material LineMaterial =>
        ammunition != null && ammunition.LineMaterial != null ? ammunition.LineMaterial : defaultMaterial;

    private Color LineColor(float alpha)
    {
        Color color = ammunition != null ? ammunition.LineColor : Color.red;
        color.a *= alpha;
        return color;
    }

    /// <summary>
    ///     Draws a solid line (beams and the sniper shot).
    /// </summary>
    private void DrawLine(Vector2 end, Color color, float width)
    {
        HideDashes();
        renderer.enabled = true;
        renderer.SetPosition(0, Muzzle);
        renderer.SetPosition(1, end);
        renderer.startColor = color;
        renderer.endColor = color;
        renderer.startWidth = width;
        renderer.endWidth = width;
    }

    /// <summary>
    ///     Draws a dotted line (aim and warning) out of short line pieces.
    /// </summary>
    private void DrawDottedLine(Vector2 end, Color color, float width)
    {
        renderer.enabled = false;

        Vector2 start = Muzzle;
        Vector2 direction = end - start;
        float length = direction.magnitude;
        if (length < 0.001f)
        {
            HideDashes();
            return;
        }
        direction /= length;

        float step = Mathf.Max(0.05f, dashLength + gapLength);
        int count = Mathf.Min(MaxDashes, Mathf.CeilToInt(length / step));

        for (int i = 0; i < count; i++)
        {
            LineRenderer dash = GetDash(i);
            float from = i * step;
            float to = Mathf.Min(from + dashLength, length);
            dash.enabled = true;
            dash.SetPosition(0, start + direction * from);
            dash.SetPosition(1, start + direction * to);
            dash.startColor = color;
            dash.endColor = color;
            dash.startWidth = width;
            dash.endWidth = width;
        }

        for (int i = count; i < dashes.Count; i++)
        {
            dashes[i].enabled = false;
        }
    }

    /// <summary>
    ///     Gets a dotted line piece, making a new one the first time it is needed.
    /// </summary>
    private LineRenderer GetDash(int index)
    {
        while (dashes.Count <= index)
        {
            GameObject dashObject = new GameObject("Dash");
            dashObject.transform.SetParent(transform, false);
            LineRenderer dash = dashObject.AddComponent<LineRenderer>();
            dash.useWorldSpace = true;
            dash.positionCount = 2;
            dash.sharedMaterial = LineMaterial;
            dash.sortingLayerID = renderer.sortingLayerID;
            dash.sortingOrder = renderer.sortingOrder;
            dashes.Add(dash);
        }
        return dashes[index];
    }

    private void HideDashes()
    {
        foreach (LineRenderer dash in dashes)
        {
            dash.enabled = false;
        }
    }

    private void HideLine()
    {
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        HideDashes();
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        // Play at the camera so the sound is always heard in 2D
        Vector3 position = Camera.main != null ? Camera.main.transform.position : transform.position;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    private Transform GetPlayer()
    {
        if (sentry != null)
        {
            return sentry.GetPlayer();
        }

        GameObject found = GameObject.FindGameObjectWithTag(tag);
        return found != null ? found.transform : null;
    }

    private Vector2 FireDirection => -transform.up;

    private Vector2 Muzzle => (Vector2)transform.position + FireDirection * muzzleDistance;

    /// <summary>
    ///     Hitscan uses the turret's direction. Tracking aims at the tracking point.
    /// </summary>
    private Vector2 BeamDirection
    {
        get
        {
            if (laserType != LaserType.Tracking)
            {
                return FireDirection;
            }

            Vector2 toPoint = trackingPoint - Muzzle;
            return toPoint.sqrMagnitude > 0.0001f ? toPoint.normalized : FireDirection;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * 3f);
    }

    [Serializable]
    private enum LaserType
    {
        Hitscan, LaserBullet, Sniper, Tracking
    }

    [Serializable]
    private enum SniperShot
    {
        Hitscan, Bullet
    }

    private enum Phase
    {
        Idle, Aiming, Locked, Firing, Cooldown
    }

    private class HitDistanceComparer : IComparer<RaycastHit2D>
    {
        public static readonly HitDistanceComparer Instance = new HitDistanceComparer();

        public int Compare(RaycastHit2D a, RaycastHit2D b) => a.distance.CompareTo(b.distance);
    }
}
