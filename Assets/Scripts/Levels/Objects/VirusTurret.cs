using System.Collections;
using PlayerController;
using UnityEngine;

/// <summary>
///     Turret that slowly sweeps back and forth. If the player enters its cone of vision,
///     it locks on and fires until the player leaves the firing radius.
/// </summary>
public class VirusTurret : MonoBehaviour
{
    private const float POWER_UP_ANIM_TIME = 1.55f;

    [SerializeField] private Transform rotatingPointTransform;

    [SerializeField] private float speed = 1f;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform shootingPoint;
    [SerializeField] private LayerMask laserCollidesWith;
    [SerializeField] private LayerMask playerLayer;

    public bool turnedOn;
    public PlayerInfo playerInfo;
    public int energyTransferPerSecond;
    public int virusTransferPerSecond = 5;

    [SerializeField][Range(POWER_UP_ANIM_TIME, 10f)] private float startDelay = POWER_UP_ANIM_TIME;
    [SerializeField][Range(POWER_UP_ANIM_TIME, 10f)] private float endDelay = POWER_UP_ANIM_TIME;
    [Tooltip("Duration in seconds that this object will shoot continuously for, i.e. length of shots")]
    [SerializeField] private float shootDuration = 2f;
    [Tooltip("Duration in seconds that this object will pause for between shooting")]
    [SerializeField][Range(POWER_UP_ANIM_TIME, 10f)] private float delayBetweenShots = POWER_UP_ANIM_TIME;
    [SerializeField] private float firingRadius = 20f;

    [Header("Surveying")]
    [Tooltip("Half-angle of the cone of vision in degrees (e.g. 15 = 30 degree total cone)")]
    [SerializeField] private float visionConeHalfAngle = 15f;
    [Tooltip("Degrees per second the turret sweeps while surveying")]
    [SerializeField] private float sweepSpeed = 30f;
    [Tooltip("How long the turret pauses at each end of its sweep")]
    [SerializeField] private float sweepPauseTime = 0.5f;
    [Tooltip("Total arc of the sweep in degrees (centered on the turret's initial facing)")]
    [SerializeField] private float sweepArc = 90f;

    public AudioSource audioSource;

    private bool activateVisual;
    private EnergyManager energyManager;
    private Coroutine laserCoroutine;
    private Coroutine sweepCoroutine;
    private Vector3 lastTargetPos;
    private Animator virusAnimator;

    // Surveying state
    private float baseAngle;         // The resting/center angle the turret was placed at
    private bool isLockedOn = false;

    private void Start()
    {
        energyManager = PlayerRef.PlayerManager.EnergyManager;
        turnedOn = true;
        lineRenderer.textureMode = LineTextureMode.Tile;
        virusAnimator = rotatingPointTransform.transform.GetChild(2).GetComponent<Animator>();
        laserCoroutine = null;

        // Capture whatever angle the turret is placed at in the editor as its sweep center
        baseAngle = rotatingPointTransform.rotation.eulerAngles.z;

        sweepCoroutine = StartCoroutine(SweepRoutine());
    }

    private void Update()
    {
        lineRenderer.gameObject.SetActive(false);

        Transform playerTransform = GetPlayerIfInNoticeRange();

        if (playerTransform != null && IsPlayerInVisionCone(playerTransform))
        {
            // Player spotted — lock on
            if (!isLockedOn)
            {
                isLockedOn = true;
                if (sweepCoroutine != null)
                {
                    StopCoroutine(sweepCoroutine);
                    sweepCoroutine = null;
                }
            }

            lastTargetPos = playerTransform.position;
            turnedOn = true;

            if (laserCoroutine == null)
                laserCoroutine = StartCoroutine(LaserAnimationCycle());

            TrackTarget(playerTransform.position);
            RenderLaserIfActive(playerTransform.position);
        }
        else if (isLockedOn && playerTransform != null)
        {
            // Player is still in firing radius but has left the vision cone — keep tracking
            // (they already triggered the turret, it doesn't just forget about them)
            lastTargetPos = playerTransform.position;
            TrackTarget(playerTransform.position);
            RenderLaserIfActive(playerTransform.position);
        }
        else
        {
            // Player not in range or lost — go back to sweeping
            if (isLockedOn)
            {
                isLockedOn = false;
                turnedOn = false;

                if (sweepCoroutine == null)
                    sweepCoroutine = StartCoroutine(SweepRoutine());
            }
        }
    }

    /// <summary>
    /// Smoothly rotates the turret toward a world-space target position.
    /// </summary>
    private void TrackTarget(Vector3 targetPos)
    {
        Vector2 direction = targetPos - rotatingPointTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatingPointTransform.rotation = Quaternion.Lerp(
            rotatingPointTransform.rotation, targetRotation, speed * Time.deltaTime);
    }

    /// <summary>
    /// Handles the line renderer display and damage logic when activateVisual is true.
    /// </summary>
    private void RenderLaserIfActive(Vector3 targetPos)
    {
        if (activateVisual && Vector3.Distance(transform.position, targetPos) < firingRadius)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                shootingPoint.position, shootingPoint.right, firingRadius, laserCollidesWith);

            lineRenderer.gameObject.SetActive(true);
            lineRenderer.SetPosition(0, shootingPoint.position);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.material.mainTextureScale = new Vector2(
                Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)), 1f);

            if (hit.transform != null &&
                LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player")
            {
                energyManager.Battery += energyTransferPerSecond * Time.fixedDeltaTime;
                energyManager.Virus += virusTransferPerSecond * Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Returns true if the player is within the turret's cone of vision
    /// (i.e. within visionConeHalfAngle degrees of the turret's current facing).
    /// </summary>
    private bool IsPlayerInVisionCone(Transform playerTransform)
    {
        Vector2 toPlayer = playerTransform.position - rotatingPointTransform.position;
        // shootingPoint.right is the direction the turret barrel is currently facing
        float angle = Vector2.Angle(shootingPoint.right, toPlayer);
        return angle <= visionConeHalfAngle;
    }

    /// <summary>
    /// Slowly sweeps the turret back and forth across its arc while no player is spotted.
    /// </summary>
    private IEnumerator SweepRoutine()
    {
        float halfArc = sweepArc / 2f;
        float minAngle = baseAngle - halfArc;
        float maxAngle = baseAngle + halfArc;
        float currentAngle = rotatingPointTransform.rotation.eulerAngles.z;
        int direction = 1; // 1 = sweeping toward maxAngle, -1 = toward minAngle

        while (true)
        {
            float targetAngle = direction == 1 ? maxAngle : minAngle;

            // Rotate toward the target sweep angle
            while (true)
            {
                currentAngle = rotatingPointTransform.rotation.eulerAngles.z;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

                if (Mathf.Abs(angleDiff) < 0.5f)
                    break;

                float step = Mathf.Sign(angleDiff) * sweepSpeed * Time.deltaTime;
                // Clamp so we don't overshoot
                if (Mathf.Abs(step) > Mathf.Abs(angleDiff))
                    step = angleDiff;

                rotatingPointTransform.rotation = Quaternion.Euler(
                    0f, 0f, currentAngle + step);

                yield return null;
            }

            // Pause at the end of the sweep
            yield return new WaitForSeconds(sweepPauseTime);

            direction *= -1; // reverse
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRadius);

        // Draw the vision cone in the scene view
        if (shootingPoint != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 forward = shootingPoint.right;
            float coneLength = firingRadius;
            float halfAngleRad = visionConeHalfAngle * Mathf.Deg2Rad;

            Vector3 leftEdge = Quaternion.Euler(0, 0, visionConeHalfAngle) * forward * coneLength;
            Vector3 rightEdge = Quaternion.Euler(0, 0, -visionConeHalfAngle) * forward * coneLength;

            Gizmos.DrawRay(shootingPoint.position, leftEdge);
            Gizmos.DrawRay(shootingPoint.position, rightEdge);
        }
    }

    private Transform GetPlayerIfInNoticeRange()
    {
        Collider2D[] desiredTargets = Physics2D.OverlapCircleAll(
            transform.position, firingRadius, playerLayer);
        foreach (Collider2D target in desiredTargets)
        {
            PlayerController2D playerController = target.GetComponent<PlayerController2D>();
            if (playerController != null)
                return target.gameObject.transform;
        }
        return null;
    }

    private IEnumerator LaserAnimationCycle()
    {
        StartPowerUpAnimation();
        bool poweredDown = false;
        yield return new WaitForSeconds(startDelay);

        while (turnedOn)
        {
            SetVirusBeamActive(true);
            StartFiringAnimation();
            yield return new WaitForSeconds(shootDuration);

            if (!turnedOn)
                break;

            SetVirusBeamActive(false);
            poweredDown = true;
            StartPowerDownAnimation();
            yield return new WaitForSeconds(endDelay);

            if (!turnedOn)
                break;

            StartPowerUpAnimation();
            poweredDown = false;
            yield return new WaitForSeconds(startDelay);
        }

        if (!poweredDown)
            StartPowerDownAnimation();

        laserCoroutine = null;
    }

    private void SetVirusBeamActive(bool isActive)
    {
        activateVisual = isActive;
        if (isActive)
            audioSource.Play();
        else
            audioSource.Stop();
    }

    private void StartPowerUpAnimation()
    {
        if (virusAnimator != null)
        {
            virusAnimator.SetBool("turnedOn", turnedOn);
            if (turnedOn)
                virusAnimator.SetInteger("firingStatus", 1);
        }
    }

    private void StartFiringAnimation()
    {
        if (virusAnimator != null)
            virusAnimator.SetInteger("firingStatus", 0);
    }

    private void StartPowerDownAnimation()
    {
        if (virusAnimator != null)
        {
            virusAnimator.SetBool("turnedOn", false);
            virusAnimator.SetInteger("firingStatus", -1);
        }
    }
}
