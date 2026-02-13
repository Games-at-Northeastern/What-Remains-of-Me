using System.Collections;
using PlayerController;
using UnityEngine;
/// <summary>
///     Script that tracks the current player and rotates towards it. It also handles
///     shooting out a line towards the player, i.e. the laser beam from the turret.
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
    [SerializeField] [Range(POWER_UP_ANIM_TIME, 10f)] private float startDelay = POWER_UP_ANIM_TIME;
    [SerializeField] [Range(POWER_UP_ANIM_TIME, 10f)] private float endDelay = POWER_UP_ANIM_TIME;
    [Tooltip("Duration in seconds that this object will shoot continuously for, i.e. length of shots")]
    [SerializeField] private float shootDuration = 2f;
    [Tooltip("Duration in seconds that this object will pause for between shooting")]
    [SerializeField] [Range(POWER_UP_ANIM_TIME, 10f)] private float delayBetweenShots = POWER_UP_ANIM_TIME;
    [SerializeField] private float firingRadius = 20;

    public AudioSource audioSource;

    private bool activateVisual;
    private EnergyManager energyManager;
    private Coroutine laserCoroutine;
    private Vector3 lastTargetPos;


    private Animator virusAnimator;

    private void Start()
    {
        energyManager = PlayerRef.PlayerManager.EnergyManager;
        turnedOn = true;
        lineRenderer.textureMode = LineTextureMode.Tile;
        virusAnimator = rotatingPointTransform.transform.GetChild(2).GetComponent<Animator>();
        laserCoroutine = null;
        // StartCoroutine(ShootLaserCycle());
    }

    private void Update()
    {
        lineRenderer.gameObject.SetActive(false);

        Transform playerTransform = GetPlayerIfInNoticeRange();

        if (playerTransform != null || virusAnimator.GetInteger("firingStatus") == 0) {
            Vector3 targetPos;
            if (playerTransform != null) {
                lastTargetPos = playerTransform.position;
                targetPos = playerTransform.position;
            } else {
                targetPos = lastTargetPos;
            }

            turnedOn = true;
            if (laserCoroutine == null) {
                laserCoroutine = StartCoroutine(LaserAnimationCycle());
            }

            // Calculate the direction towards the player, and rotate that way
            Vector2 direction = targetPos - rotatingPointTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rotatingPointTransform.rotation = Quaternion.Lerp(rotatingPointTransform.rotation, targetRotation, speed * Time.deltaTime);
            //turretTransform.rotation = targetRotation;

            if (activateVisual && Vector3.Distance(transform.position, targetPos) < firingRadius) {

                // Determine the nearest collision from the turret's shooting line towards the player,
                // and set the line renderer to display until that collision point
                RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingPoint.right, firingRadius, laserCollidesWith);
                // Debug.Log(hit.collider.gameObject); why is this left in here? it errors.

                //Render the laser
                lineRenderer.gameObject.SetActive(true);
                lineRenderer.SetPosition(0, shootingPoint.position);
                lineRenderer.SetPosition(1, hit.point);

                // Set the scale of the texture to the magnitude of the line being rendered, so that there's no
                // texture squishing or stretching
                lineRenderer.material.mainTextureScale = new Vector2(Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)), 1f);

                // If the shooting line/laser hits the player, manually adjust the player's energy and virus appropriately
                if (hit.transform != null && LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player") // this also tends to error? is this a setup issue?
                {
                    energyManager.Battery += energyTransferPerSecond * Time.fixedDeltaTime;

                    energyManager.Virus += virusTransferPerSecond * Time.fixedDeltaTime;
                }
            }
        } else {
            turnedOn = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRadius);
    }

    private Transform GetPlayerIfInNoticeRange()
    {
        // Getting all layer objects players, just in case there are multiple things with the player layer
        Collider2D[] desiredTargets = Physics2D.OverlapCircleAll(transform.position, firingRadius, playerLayer);
        foreach (Collider2D target in desiredTargets) {
            PlayerController2D playerController = target.GetComponent<PlayerController2D>();
            if (playerController != null) {
                return target.gameObject.transform;
            }
        }
        return null;
    }
    private IEnumerator LaserAnimationCycle()
    {
        StartPowerUpAnimation();
        bool poweredDown = false;
        yield return new WaitForSeconds(startDelay);

        while (turnedOn) {
            SetVirusBeamActive(true);
            StartFiringAnimation();
            yield return new WaitForSeconds(shootDuration);

            if (!turnedOn) {
                break;
            }

            // pause shooting for <delayBetweenShots> seconds
            // for animations to work delayBetweenShots has to be at least 1.55 seconds
            SetVirusBeamActive(false);
            poweredDown = true;
            StartPowerDownAnimation();
            yield return new WaitForSeconds(endDelay);

            if (!turnedOn) {
                break;
            }

            StartPowerUpAnimation();
            poweredDown = false;
            yield return new WaitForSeconds(startDelay);

        }

        if (!poweredDown) {
            StartPowerDownAnimation();
        }

        laserCoroutine = null;
    }

    private void SetVirusBeamActive(bool isActive)
    {
        activateVisual = isActive;
        if (isActive) {
            audioSource.Play();
        } else {
            audioSource.Stop();
        }
    }

    // Functions for animations for the virus laser
    private void StartPowerUpAnimation()
    {
        // Support for virus laser turning on/off and animation turning on/off with it if laser has an animator
        if (virusAnimator != null) {
            if (turnedOn) {
                virusAnimator.SetBool("turnedOn", true);
                virusAnimator.SetInteger("firingStatus", 1);
            } else {
                virusAnimator.SetBool("turnedOn", false);
            }
        }
    }

    private void StartFiringAnimation()
    {
        if (virusAnimator != null) {
            virusAnimator.SetInteger("firingStatus", 0);
        }
    }

    private void StartPowerDownAnimation()
    {
        if (virusAnimator != null) {
            if (turnedOn) {
                virusAnimator.SetBool("turnedOn", false);
                virusAnimator.SetInteger("firingStatus", -1);
            } else {
                virusAnimator.SetBool("turnedOn", false);
                virusAnimator.SetInteger("firingStatus", -1);
            }
        }
    }
}
