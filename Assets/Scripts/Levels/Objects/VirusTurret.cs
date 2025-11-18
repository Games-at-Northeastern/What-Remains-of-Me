using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that tracks the current player and rotates towards it. It also handles
/// shooting out a line towards the player, i.e. the laser beam from the turret.
/// </summary>
public class VirusTurret : MonoBehaviour
{

    [SerializeField] private Transform rotatingPointTransform;


    [SerializeField] private Transform playerTransform;

    [SerializeField] private float speed = 1f;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform shootingPoint;
    [SerializeField] private LayerMask laserCollidesWith;



    public bool turnedOn;
    public PlayerInfo playerInfo;
    public int energyTransferPerSecond = 0;
    public int virusTransferPerSecond = 5;

    [SerializeField] private float startDelay = 1f;
    [Tooltip("Duration in seconds that this object will shoot continuously for, i.e. length of shots")]
    [SerializeField] private float shootDuration = 2f;
    [Tooltip("Duration in seconds that this object will pause for between shooting")]
    [SerializeField] private float delayBetweenShots = 2f;
    [SerializeField] private float maxLaserDistance = 20;

    public AudioSource audioSource;

    private bool activateVisual = false;
    private Animator virusAnimator;

    private void Start()
    {
        turnedOn = true;
        lineRenderer.textureMode = LineTextureMode.Tile;
        virusAnimator = rotatingPointTransform.transform.GetChild(2).GetComponent<Animator>();
        StartCoroutine(ShootLaserCycle());
    }

    private void Update()
    {

        lineRenderer.gameObject.SetActive(false);

        if (turnedOn)
        {
            // Calculate the direction towards the player, and rotate that way
            Vector2 direction = playerTransform.position - rotatingPointTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rotatingPointTransform.rotation = Quaternion.Lerp(rotatingPointTransform.rotation, targetRotation, speed * Time.deltaTime);
            //turretTransform.rotation = targetRotation;

            if (activateVisual && Vector3.Distance(shootingPoint.position, playerTransform.position) < maxLaserDistance)
            {

                // Determine the nearest collision from the turret's shooting line towards the player,
                // and set the line renderer to display until that collision point
                RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingPoint.right, maxLaserDistance, laserCollidesWith);
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
                    playerInfo.battery += energyTransferPerSecond * Time.fixedDeltaTime;
                    playerInfo.virus += virusTransferPerSecond * Time.fixedDeltaTime;
                }
            }
        }
    }

    private IEnumerator ShootLaserCycle()
    {
        //Delay before beginning of the laser cycle
        //Wait until 1.55 seconds are left on startDelay to play animation if possible
        if (startDelay < 1.55f)
        {
            StartPowerUpAnimation();
            yield return new WaitForSeconds(startDelay); 
        } else 
        {
            yield return new WaitForSeconds(startDelay - 1.55f);
            StartPowerUpAnimation();
            yield return new WaitForSeconds(1.55f);
        }

        while (true)
        {
            if (turnedOn)
            {
                // begin shooting for <shootDuration> seconds
                SetVirusBeamActive(true);
                StartFiringAnimation();
                yield return new WaitForSeconds(shootDuration);

                // pause shooting for <delayBetweenShots> seconds
                // for animations to work delayBetweenShots has to be at least 1.55 seconds
                SetVirusBeamActive(false);
                StartPowerDownAnimation();
                yield return new WaitForSeconds(delayBetweenShots - 1.55f);
                
                StartPowerUpAnimation();
                yield return new WaitForSeconds(1.55f);
            } else
            {
                yield return new WaitForSeconds(delayBetweenShots);
            }
        }
    }

    private void OnDrawGizmos()
    {
        
    }

    private void SetVirusBeamActive(bool isActive)
    {
        activateVisual = isActive;
        if (isActive)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    // Functions for animations for the virus laser
    private void StartPowerUpAnimation()
    {
        // Support for virus laser turning on/off and animation turning on/off with it if laser has an animator
        if (virusAnimator != null)
        {
            if (turnedOn)
            {
                virusAnimator.SetBool("turnedOn", true);
                virusAnimator.SetInteger("firingStatus", 1);
            } else 
            {
                virusAnimator.SetBool("turnedOn", false);
            }
        }
    }

    private void StartFiringAnimation()
    {
        if (virusAnimator != null)
        {
            virusAnimator.SetInteger("firingStatus", 0);
        }
    }

    private void StartPowerDownAnimation()
    {
        if (virusAnimator != null)
        {
            if (turnedOn)
            {
                virusAnimator.SetBool("turnedOn", true);
                virusAnimator.SetInteger("firingStatus", -1);
            } else 
            {
                virusAnimator.SetBool("turnedOn", false);
            }
        }
    }
}
