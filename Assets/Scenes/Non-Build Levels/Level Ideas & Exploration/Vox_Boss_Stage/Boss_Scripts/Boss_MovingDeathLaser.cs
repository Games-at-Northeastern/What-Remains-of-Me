using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(LineRenderer))]

public class Boss_MovingDeathLaser : MonoBehaviour
{

    private const int PlayerHierarchyLayerIndex = 3;

    [SerializeField]
    [Tooltip("Sprite Laser on. ")]
    private SpriteRenderer onSprite;
    [SerializeField]
    [Tooltip("Sprite Laser off. ")]
    private SpriteRenderer offSprite;
    [SerializeField]
    [Tooltip("Light on. ")]
    private Light2D onLight;

    [SerializeField]
    [Tooltip("Laser stops when obstacle is hit. ")]
    private LaserMode laserMode;
    [SerializeField]
    [Tooltip("What the laser will be able to hit --> default and player always. ")]
    private LayerMask mask;
    [SerializeField]
    [Tooltip("All the assets that are related to firing the laser.")]
    private Particles particles;
    [SerializeField]
    [Tooltip("All the assets related to Atlas dying.")]
    private Death death;

    [SerializeField]
    [Tooltip("How far the laser will travel before stopping (only for Distance LaserMode!)")]
    private float laserDistance;

    [SerializeField]
    [Tooltip("How smoothly the laser jumps from one position to the next.")]
    [Range(1f, 250f)]
    private float resetSpeed;

    [Header("Rotation")]
    [SerializeField]
    [Tooltip("If true, the laser will continuously rotate while firing.")]
    private bool sweepEnabled = false;
    [SerializeField]
    [Tooltip("How fast the laser rotates, in degrees per second. Pos = counter-clockwise, Neg = clockwise.")]
    private float sweepSpeed = 20f;

    [Header("Projectile Mode")]
    [SerializeField]
    [Tooltip("If true, the turret fires projectiles instead of a hitscan beam.")]
    private bool useProjectiles = true;
    [SerializeField]
    [Tooltip("Seconds between shots in projectile mode.")]
    private float projectileFireInterval = 1f;
    [SerializeField]
    [Tooltip("How far in front of the turret the projectile appears.")]
    private float projectileMuzzleDistance = 0.8f;
    [SerializeField]
    [Tooltip("Speed, range (shorter than the beam), size and look of the projectiles.")]
    private Boss_CannonProjectile.Settings projectileSettings = new Boss_CannonProjectile.Settings
    {
        speed = 5f,
        range = 9f,
        radius = 0.3f,
        color = new Color(1f, 0.25f, 0.25f)
    };
    private float projectileTimer;

    private float currentLaserDistance;

    private DeathLaserSound dlsScript;
    private bool laserOn = true; 
    private bool lockout; 

    private LineRenderer renderer; 
    private readonly string tag = "Player";

    private void Awake()
    {
        renderer = GetComponent<LineRenderer>();
        dlsScript = GetComponent<DeathLaserSound>();
    }

    private void Update()
    {
        if (useProjectiles)
        {
            UpdateProjectileMode();
            return;
        }

        Vector3 laserTarget;

        if (laserOn)
        {
            // Rotate the laser before doing the raycast so the beam reflects the new rotation this frame
            if (sweepEnabled)
            {
                UpdateSweep();
            }

            // Calculates the laser's target
            if (RaycastHit(out RaycastHit2D raycastHitData)) /* If the laser hit any target */
            {
                // Draw the laser at the collison point
                laserTarget = raycastHitData.point;
            }
            else if (laserMode == LaserMode.Distance) /* If the laser didn't hit a target, and the LaserMode is a fixed distance */
            {
                // Draw the laser at the fixed distance away from the laser (factors in rotation of laser)
                laserTarget = transform.position + -transform.up * laserDistance;
            }
            else /* Laser didn't hit anything */
            {
                laserTarget = transform.position;
            }

            // Calculates the target's distance from the laser
            float laserTargetDistance = Vector3.Distance(transform.position, laserTarget);

            // If the laser has to move towards the laser target
            if (currentLaserDistance <= laserTargetDistance)
            {
                currentLaserDistance = Mathf.MoveTowards(currentLaserDistance, laserTargetDistance, resetSpeed * Time.deltaTime);
            }
            else /*If the laser is backwards to hit a target */
            {
                currentLaserDistance = laserTargetDistance;
            }

            // Calculate the target based on the distance, and draw the laser
            Vector3 currentLaserTarget = transform.position + -transform.up * currentLaserDistance;
            DrawLaser(currentLaserTarget);

            // If the current lasers distance is close to the target, and the raycast hit the player
            if (laserTargetDistance - currentLaserDistance <= 0.1)
            {
                CheckForDeath(raycastHitData); // See if Atlas died
            }
        }
        else
        {
            UndrawLaser(); // Remove laser
        }

    }

    /// <summary>
    ///     Projectile mode: no beam. Rotates (if sweep is on) and fires a projectile every interval.
    /// </summary>
    private void UpdateProjectileMode()
    {
        UndrawLaser();

        if (!laserOn)
        {
            return;
        }

        if (sweepEnabled)
        {
            UpdateSweep();
        }

        projectileTimer -= Time.deltaTime;
        if (projectileTimer > 0f)
        {
            return;
        }

        projectileTimer = projectileFireInterval;
        Vector2 direction = -transform.up;
        Vector2 muzzle = (Vector2)transform.position + direction * projectileMuzzleDistance;
        Boss_CannonProjectile.Spawn(muzzle, direction, projectileSettings, true, false, 0f, death.DeathTeleporter);
    }


    private void UpdateSweep()
    {
        float step = sweepSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, step);
    }


#if UNITY_EDITOR
    //Makes sure that the mask always has the player and default layers selected
    private void OnValidate() => mask = mask | LayerMask.GetMask("Player", "Default");
#endif

    /// <summary>
    ///     Turns the laser on or off
    /// </summary>
    public void ToggleLaser()
    {
        laserOn = !laserOn; // Turn laser on or off
        if (laserOn)
        {
            DeathLaserSound.PlayLaserOnSound(transform);
            onSprite.enabled = true;
            onLight.enabled = true;
            offSprite.enabled = false;
        }
        if (!laserOn)
        {
            DeathLaserSound.PlayLaserOffSound(transform);
            onSprite.enabled = false;
            onLight.enabled = false;
            offSprite.enabled = true;
        }
    }

    private void UndrawLaser()
    {
        if (renderer != null)
        {
            renderer.enabled = false; // Remove laser beam
        }
        SetParticleActive(particles.StartParticle, false); // Remove start particle
        SetParticleActive(particles.EndParticle, false); // Remove end particle
        SetParticleActive(particles.EmissionParticle, false); // Remove emission particle
    }

    private static void SetParticleActive(ParticleSystem particle, bool active)
    {
        if (particle != null && particle.gameObject.activeSelf != active)
        {
            particle.gameObject.SetActive(active);
        }
    }

    private void DrawLaser(Vector3 laserTarget)
    {
        // Turn on laser beam and set it's points
        renderer.enabled = true;
        renderer.SetPositions(new[]
        { transform.position, laserTarget });

        // Draw the start particle
        particles.StartParticle.gameObject.SetActive(true);
        particles.StartParticle.transform.position = transform.position - transform.rotation * particles.StartParticleOffset;

        if (particles.DisplayLaserEnd) /* If the end particle should be displayed */
        {
            // Draw the end particle
            particles.EndParticle.gameObject.SetActive(true);
            particles.EndParticle.transform.position = laserTarget + (transform.position - laserTarget).normalized * 0.1f + transform.rotation * particles.EndParticleOffset;

            // Draw the emission particles
            particles.EmissionParticle.gameObject.SetActive(true);
            particles.EmissionParticle.transform.SetPositionAndRotation(laserTarget + (transform.position - laserTarget).normalized * 0.1f, transform.rotation);
        }
    }

    private bool RaycastHit(out RaycastHit2D raycastHitData)
    {
        // Get the max distance of the raycast based on the LaserMode
        float raycastDistance = laserMode == LaserMode.Distance ? laserDistance : float.MaxValue;

        raycastHitData = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, mask); // Do Raycast
        return raycastHitData.collider != null; // Return whether the raycast hit an object
    }

    private void CheckForDeath(RaycastHit2D raycastHitData)
    {

        if (!lockout
            && raycastHitData.collider != null
            && !PlayerRef.PlayerManager.GodMode
            && UtilityFunctions.CompareTagOfHierarchy(raycastHitData.collider.transform, tag, out Transform player, PlayerHierarchyLayerIndex))
        {
            // Teleport Atlas
            StartCoroutine(death.DeathTeleporter.PerformDeath(player.gameObject));
            // Play death sound
            SoundController.instance.PlaySound("Laser_Death_Sound");
            // Start lockout time
            lockout = true;
            Invoke(nameof(LockoutCooldownInvocation), death.LockoutTime);
        }
    }

    // IEnumerators are an optimzation for the future.
    private void LockoutCooldownInvocation() => lockout = false;

    [Serializable]
    private enum LaserMode
    {
        Raycast, Distance
    }

    [Serializable]
    private struct Particles
    {
        public ParticleSystem StartParticle;
        [Tooltip("The offset in the vector that makes the Start Particle lie in the center")]
        public Vector3 StartParticleOffset;

        [Tooltip("Does the laser want to show the end particle?")]
        public bool DisplayLaserEnd;
        public ParticleSystem EndParticle;
        [Tooltip("The offset in the vector that makes the End Paricle lie in the center")]
        public Vector3 EndParticleOffset;
        public ParticleSystem EmissionParticle;
    }

    [Serializable]
    private struct Death
    {
        public SpikeTeleport DeathTeleporter;
        [Range(1.5f, 10f)]
        [Tooltip("The amount of time before the laser could hit Atlas again!")]
        public float LockoutTime;
    }
}
