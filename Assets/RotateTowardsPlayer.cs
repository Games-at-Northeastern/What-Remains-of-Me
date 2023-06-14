using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that tracks the current player and rotates towards it. It also handles
/// shooting out a line towards the player, i.e. the laser beam from the turret.
/// </summary>
public class RotateTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform turretTransform;

    [SerializeField] private Transform playerTransform;

    [SerializeField] private float speed = 10f;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform shootingPoint;


    public PlayerInfo playerInfo;
    public int energyTransferPerSecond = 0;
    public int virusTransferPerSecond = 5;

    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float repeatRate = 2f;

    private bool activateVisual = true;

    private void Start()
    {
        lineRenderer.textureMode = LineTextureMode.Tile;

        // Invoke the function every <repeatRate> seconds, starting after <startDelay> seconds
        InvokeRepeating("ToggleVirusBeamActive", startDelay, repeatRate);

    }


    private void Update()
    {

        // Calculate the direction towards the player, and rotate that way
        Vector2 direction = playerTransform.position - turretTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set the scale of the texture to the magnitude of the line being rendered, so that there's no
        // texture squishing or stretching
        lineRenderer.material.mainTextureScale = new Vector2 (Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)), 1f);

        if (activateVisual)
        {

            // Determine the nearest collision from the turret's shooting line towards the player,
            // and set the line renderer to display until that collision point
            RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingPoint.right);
            lineRenderer.SetPosition(0, shootingPoint.position);
            lineRenderer.SetPosition(1, hit.point);


            // If the shooting line/laser hits the player, manually adjust the player's energy and virus appropriately
            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player")
            {
                playerInfo.battery += energyTransferPerSecond * Time.fixedDeltaTime;
                playerInfo.virus += virusTransferPerSecond * Time.fixedDeltaTime;
            }

        }


    }


    private void ToggleVirusBeamActive()
    {
        activateVisual = !activateVisual;
        lineRenderer.gameObject.SetActive(activateVisual);
    }


}
