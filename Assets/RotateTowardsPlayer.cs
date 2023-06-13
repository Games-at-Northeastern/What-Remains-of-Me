using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform turretTransform;

    [SerializeField] private Transform playerTransform;

    [SerializeField] private float speed = 10f;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform shootingPoint;


    public PlayerInfo playerInfo;
    public int energyAmount = 0;
    public int virusAmount = 5;

    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float repeatRate = 2f;

    private bool activateVisual = true;

    private void Start()
    {
        lineRenderer.textureMode = LineTextureMode.Tile;

        // Invoke the function every 2 seconds, starting after 1 second
        InvokeRepeating("SetVirusBeamActive", startDelay, repeatRate);
    }


    private void Update()
    {

        float angle = Mathf.Atan2(playerTransform.position.y - turretTransform.position.y, playerTransform.position.x - turretTransform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        turretTransform.rotation = Quaternion.RotateTowards(turretTransform.rotation, targetRotation, speed * Time.deltaTime);

        lineRenderer.material.mainTextureScale = new Vector2 (Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)), 1f);

        Debug.Log(activateVisual);

        if (activateVisual)
        {


 



            RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingPoint.right);
            lineRenderer.SetPosition(0, shootingPoint.position);
            lineRenderer.SetPosition(1, hit.point);


            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player")
            {
                playerInfo.battery += energyAmount * Time.fixedDeltaTime;
                playerInfo.virus += virusAmount * Time.fixedDeltaTime;
            }

        }


    }


    private void SetVirusBeamActive()
    {
        activateVisual = !activateVisual;
        lineRenderer.gameObject.SetActive(!lineRenderer.gameObject.activeInHierarchy);
    }


}
