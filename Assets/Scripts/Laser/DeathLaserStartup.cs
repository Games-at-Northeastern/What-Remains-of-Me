using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class DeathLaserStartup : MonoBehaviour
{
    [System.Serializable]
    private enum LaserMode
    {
        Raycast, Distance
    }

    [System.Serializable]
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

        [Tooltip("Each vector2 represents a single flicker. The x-value is the delay prefacing the flicker, the y-value is the duration of the flicker")]
        public List<Vector2> DelayDurationValues;
    }

    [SerializeField, Tooltip("Raycast: The laser travels until it hits an obstacle.\nFixed: The laser stops at a set distance.")]
    private LaserMode laserMode;
    [SerializeField, Tooltip("How far the laser will travel before stopping (only for Distance LaserMode!)")]
    private float laserDistance;
    [SerializeField, Tooltip("All the assets that are related to firing the laser.")]
    private Particles particles;
    [SerializeField, Tooltip("How smoothly the laser jumps from one position to the next."), Range(1f, 250f)]
    private float resetSpeed;
    [SerializeField, Tooltip("What the laser will be able to hit (make sure default and player are always included!)")]
    private LayerMask mask;

    private float currentLaserDistance = 0.0f;
    private LineRenderer renderer; // Laser beam

    private void Start()
    {
        renderer = GetComponent<LineRenderer>();
    }

    public void StartupAnimation()
    {
        StartCoroutine(PlayStartupAnimation());
    }

    public void ShutdownAnimation()
    {
        StartCoroutine(PlayShutdownAnimation());
    }

    private IEnumerator PlayStartupAnimation()
    {
        Vector3 laserTarget;

        // Calculates the laser's target
        if (RaycastHit(out RaycastHit2D raycastHitData)) /* If the laser hit any target */
        {
            // Draw the laser at the collison point
            laserTarget = raycastHitData.point;
        }
        else if (laserMode == LaserMode.Distance) /* If the laser didn't hit a target, and the LaserMode is a fixed distance */
        {
            // Draw the laser at the fixed distance away from the laser (factors in rotation of laser)
            laserTarget = transform.position + (-transform.up * laserDistance);
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
        Vector3 currentLaserTarget = transform.position + (-transform.up * currentLaserDistance);

        // delay and draw logic for each instance of a laser delay and duration pairing in flickerValues
        foreach (Vector2 laser in particles.DelayDurationValues)
        {
            yield return new WaitForSeconds(laser.x);
            DrawLaser(currentLaserTarget);
            yield return new WaitForSeconds(laser.y);
            UndrawLaser();
        }
    }

    private void DrawLaser(Vector3 laserTarget)
    {
        // Turn on laser beam and set it's points
        renderer.enabled = true;
        renderer.SetPositions(new Vector3[] { transform.position, laserTarget });

        // Draw the start particle
        particles.StartParticle.gameObject.SetActive(true);
        particles.StartParticle.transform.position = transform.position - (transform.rotation * particles.StartParticleOffset);

        if (particles.DisplayLaserEnd) /* If the end particle should be displayed */
        {
            // Draw the end particle
            particles.EndParticle.gameObject.SetActive(true);
            particles.EndParticle.transform.position = laserTarget + ((transform.position - laserTarget).normalized * 0.1f) + (transform.rotation * particles.EndParticleOffset);

            // Draw the emission particles
            particles.EmissionParticle.gameObject.SetActive(true);
            particles.EmissionParticle.transform.SetPositionAndRotation(laserTarget + ((transform.position - laserTarget).normalized * 0.1f), transform.rotation);
        }
    }

    /// <summary>
    /// Removes the laser from the screen
    /// </summary>
    private void UndrawLaser()
    {
        renderer.enabled = false;                                   // Remove laser beam
        particles.StartParticle.gameObject.SetActive(false);        // Remove start particle
        particles.EndParticle.gameObject.SetActive(false);          // Remove end particle
        particles.EmissionParticle.gameObject.SetActive(false);     // Remove emission particle
    }

    private IEnumerator PlayShutdownAnimation()
    {
        Vector3 laserTarget;

        // Calculates the laser's target
        if (RaycastHit(out RaycastHit2D raycastHitData)) /* If the laser hit any target */
        {
            // Draw the laser at the collison point
            laserTarget = raycastHitData.point;
        }
        else if (laserMode == LaserMode.Distance) /* If the laser didn't hit a target, and the LaserMode is a fixed distance */
        {
            // Draw the laser at the fixed distance away from the laser (factors in rotation of laser)
            laserTarget = transform.position + (-transform.up * laserDistance);
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
        Vector3 currentLaserTarget = transform.position + (-transform.up * currentLaserDistance);

        // delay and draw logic for each instance of a laser delay and duration pairing in flickerValues
        foreach (Vector2 laser in particles.DelayDurationValues)
        {
            yield return new WaitForSeconds(laser.x);
            UndrawLaser();
            yield return new WaitForSeconds(laser.y);
            DrawLaser(currentLaserTarget);
        }
    }

    /// <summary>
    /// Performs a raycast for the laser based on it's current rotation!
    /// </summary>
    /// <param name="raycastHitData"> Raycast data </param>
    /// <returns> Returns whether the raycast hit an object </returns>
    private bool RaycastHit(out RaycastHit2D raycastHitData)
    {
        // Get the max distance of the raycast based on the LaserMode
        float raycastDistance = laserMode == LaserMode.Distance ? laserDistance : float.MaxValue;

        raycastHitData = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, mask); // Do Raycast
        return raycastHitData.collider != null; // Return whether the raycast hit an object
    }
}
