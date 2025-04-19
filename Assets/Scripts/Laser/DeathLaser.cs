using UnityEngine;

/// <summary>
/// This is the script that controls the RAD (Rapid Atlas Disassembly) laser.
///
/// There are 2 modes for the laser:
/// 1. Raycast
/// 2. Fixed
///
/// Raycast casts a raycast from the current position and rotation of the laser until it collides with the player or obstacles.
///
/// Fixed casts a raycast from the current postion and rotation of the laser, but has a max distance that the laser can travel!
/// 
/// </summary>
///

[RequireComponent(typeof(LineRenderer))]
public class DeathLaser : MonoBehaviour
{

    [System.Serializable]
    private enum LaserMode {
        Raycast, Distance
    }

    [System.Serializable]
    private struct Particles {
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

    [System.Serializable]
    private struct Death {
        public SpikeTeleport DeathTeleporter;
        [Range(1.5f, 10f), Tooltip("The amount of time before the laser could hit Atlas again!")]
        public float LockoutTime;
    }

    [SerializeField, Tooltip("Raycast: The laser travels until it hits an obstacle.\nFixed: The laser stops at a set distance.")]
    private LaserMode laserMode;
    [SerializeField, Tooltip("What the laser will be able to hit (make sure default and player are always included!)")]
    private LayerMask mask;
    [SerializeField, Tooltip("All the assets that are related to firing the laser.")]
    private Particles particles;
    [SerializeField, Tooltip("All the assets related to Atlas dying.")]
    private Death death;

    [SerializeField, Tooltip("How far the laser will travel before stopping (only for Distance LaserMode!)")]
    private float laserDistance;

    [SerializeField, Tooltip("How smoothly the laser jumps from one position to the next."), Range(1f, 250f)]
    private float resetSpeed;

    private LineRenderer renderer; // Laser beam
    private bool lockout = false;  // Death timeout for player
    private bool laserOn = true;   // If the laser is on or off

    private float currentLaserDistance = 0.0f;

    private DeathLaserSound dlsScript;

    private void Awake()
    {
        renderer = GetComponent<LineRenderer>();
        dlsScript = GetComponent<DeathLaserSound>();
    }

    /// <summary>
    /// Casts a raycast of the laser every frame, and kills Atlas if he collided with the laser!
    /// </summary>
    private void Update() {
        Vector3 laserTarget;

        if (laserOn) {
            // Calculates the laser's target
            if (RaycastHit(out RaycastHit2D raycastHitData)) /* If the laser hit any target */{
                // Draw the laser at the collison point
                laserTarget = raycastHitData.point; 
            } else if (laserMode == LaserMode.Distance) /* If the laser didn't hit a target, and the LaserMode is a fixed distance */ {
                // Draw the laser at the fixed distance away from the laser (factors in rotation of laser)
                laserTarget = transform.position + (-transform.up * laserDistance);
            } else /* Laser didn't hit anything */ {
                laserTarget = transform.position;
            }

            // Calculates the target's distance from the laser
            float laserTargetDistance = Vector3.Distance(transform.position, laserTarget);

            // If the laser has to move towards the laser target
            if (currentLaserDistance <= laserTargetDistance) {
                currentLaserDistance = Mathf.MoveTowards(currentLaserDistance, laserTargetDistance, resetSpeed * Time.deltaTime);
            } else /*If the laser is backwards to hit a target */ {
                currentLaserDistance = laserTargetDistance;
            }

            // Calculate the target based on the distance, and draw the laser
            Vector3 currentLaserTarget = transform.position + (-transform.up * currentLaserDistance);
            DrawLaser(currentLaserTarget);

            // If the current lasers distance is close to the target, and the raycast hit the player
            if (laserTargetDistance - currentLaserDistance <= 0.1) {
                CheckForDeath(raycastHitData); // See if Atlas died
            }
        } else {
            UndrawLaser(); // Remove laser
        }

    }

    /// <summary>
    /// Turns the laser on or off
    /// </summary>
    public void ToggleLaser() {
        laserOn = !laserOn; // Turn laser on or off
        if (laserOn)
        {
            DeathLaserSound.PlayLaserOnSound(transform);
        }
        if (!laserOn)
        {
            DeathLaserSound.PlayLaserOffSound(transform);
        }
    }

    /// <summary>
    /// Removes the laser from the screen
    /// </summary>
    private void UndrawLaser() {
        renderer.enabled = false;                                   // Remove laser beam
        particles.StartParticle.gameObject.SetActive(false);        // Remove start particle
        particles.EndParticle.gameObject.SetActive(false);          // Remove end particle
        particles.EmissionParticle.gameObject.SetActive(false);     // Remove emission particle
    }

    /// <summary>
    /// Draws the laser to the screen!
    /// </summary>
    /// <param name="laserTarget"> The endpoint of the laser (start is the GameObject's position) </param>
    private void DrawLaser(Vector3 laserTarget) {
        // Turn on laser beam and set it's points
        renderer.enabled = true;
        renderer.SetPositions(new Vector3[] { transform.position, laserTarget });

        // Draw the start particle
        particles.StartParticle.gameObject.SetActive(true);
        particles.StartParticle.transform.position = transform.position - (transform.rotation * particles.StartParticleOffset);

        if (particles.DisplayLaserEnd) /* If the end particle should be displayed */ {
            // Draw the end particle
            particles.EndParticle.gameObject.SetActive(true);
            particles.EndParticle.transform.position = laserTarget + ((transform.position - laserTarget).normalized * 0.1f) + (transform.rotation * particles.EndParticleOffset);

            // Draw the emission particles
            particles.EmissionParticle.gameObject.SetActive(true);
            particles.EmissionParticle.transform.SetPositionAndRotation(laserTarget + ((transform.position - laserTarget).normalized * 0.1f), transform.rotation);
        }
    }

    /// <summary>
    /// Performs a raycast for the laser based on it's current rotation!
    /// </summary>
    /// <param name="raycastHitData"> Raycast data </param>
    /// <returns> Returns whether the raycast hit an object </returns>
    private bool RaycastHit(out RaycastHit2D raycastHitData) {
        // Get the max distance of the raycast based on the LaserMode
        float raycastDistance = laserMode == LaserMode.Distance ? laserDistance : float.MaxValue;

        raycastHitData = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, mask); // Do Raycast
        return raycastHitData.collider != null; // Return whether the raycast hit an object
    }

    private const int PlayerHierarchyLayerIndex = 3;
    private string tag = "Player";
    /// <summary>
    /// Determines whether Atlas died in the current Raycast
    /// </summary>
    /// <param name="raycastHitData"> Raycast data </param>
    private void CheckForDeath(RaycastHit2D raycastHitData) {
        // The player has several colliders on their gameobject, somewhere in the hierarchy. Because of this, the death effect can get
        // triggered several times (that's the purpose of lockout). Additionally, only the capsule collider is on a gameobject tagged
        // as the player; the rest are untagged children. Therefore, I need to look through all the parents (limited to 3, since that's
        // the max relevant hierachy level) to find the player tag. This is very sad. Oh well.
        if (!lockout
            && raycastHitData.collider != null
            && UtilityFunctions.CompareTagOfHierarchy(raycastHitData.collider.transform, tag, out var player, PlayerHierarchyLayerIndex)) {
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


#if UNITY_EDITOR
    //Makes sure that the mask always has the player and default layers selected
    private void OnValidate() => mask = mask | LayerMask.GetMask("Player", "Default");
#endif
}
