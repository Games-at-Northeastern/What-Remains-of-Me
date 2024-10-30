using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class PlayerDetector : MonoBehaviour
{
    public Light2D detectCone;
    private bool isColliding = false;
    private Vector3 cPos;
    private float detectConeAngle;
    private float detectConePartition;
    private float detectDistance;
    private float facingAngle;
    private PolygonCollider2D pc;


    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PolygonCollider2D>();
    }
    private void Update()
    {
        cPos = detectCone.transform.position;
        detectConeAngle = detectCone.pointLightOuterAngle;
        detectDistance = detectCone.pointLightOuterRadius;
        ColliderConstruction();

        /*RaycastHit2D hit = Physics2D.Raycast(detectCone.transform.position, detectCone.transform.forward, 500f);
        if (hit)
        {
            Debug.Log("Hit " + hit.collider.name);
            Debug.DrawRay(detectCone.transform.position, detectCone.transform.TransformDirection(Vector2.up) * 10f, Color.blue);
        }
        //checkForColliders(ray1);
        */
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("triggered");

        RaycastHit2D hit = Physics2D.Raycast(detectCone.transform.position, new Vector2(other.transform.position.x,other.transform.position.y), detectDistance);
        if (hit.collider!=null)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Hit " + other.name);
                Debug.DrawRay(detectCone.transform.position, detectCone.transform.TransformDirection(Vector2.up) * 10f, Color.blue);
            }
            else
            {
                return;
            }
        }
        //checkForColliders(ray1);
    }
    private void ColliderConstruction()
    {
        Vector2 origin = new Vector2(0,0);
        Vector2 centerPoint = new Vector2(0, detectDistance);
        Vector2[] vertices = new Vector2[]
        {
            new Vector2(0, 0), // origin
            new Vector2(1, 0), // right
            new Vector2(0, 1), // left
        };

        float totalAngle = detectConeAngle;
        float halfAngle = totalAngle * .5f;
        float compAngle = 90f - halfAngle;
        float halfRad = halfAngle * Mathf.Deg2Rad;
        float rightHypotenuse = detectDistance/Mathf.Cos(halfRad);
        float rightOpposite = rightHypotenuse*Mathf.Sin(halfRad);
        Vector2 rightVert = new Vector2(detectDistance, rightOpposite);
        Vector2 leftVert = new Vector2(detectDistance, -rightOpposite);
        vertices[1] = rightVert;
        vertices[2] = leftVert;
        pc.points = vertices;
    }
}
