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

    private float startRotationX;
    private float startRotationY;

    private void Start()
    {
        lineRenderer.textureMode = LineTextureMode.Tile;

        // Invoke the function every 2 seconds, starting after 1 second
        InvokeRepeating("SetVirusBeamActive", startDelay, repeatRate);

        startRotationX = turretTransform.rotation.x;
        startRotationY = turretTransform.rotation.y;
    }


    private void Update()
    {


        Vector2 direction = playerTransform.position - turretTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);



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
