using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerDetector : MonoBehaviour
{
    #region player tracking
    [Header("Player Tracking")]
    [SerializeField] private GameObject detectedObject;
    [SerializeField] private Collider2D detectedCollider;
    [SerializeField] private bool isTracking;
    [SerializeField] private GameObject what;
    private bool isColliding = false;
    #endregion

    #region collider variables
    [Header("Collider variables")]
    public Light2D detectCone;
    private Vector3 cPos;
    private float detectConeAngle;
    private float detectConePartition;
    [SerializeField] private float detectDistance;
    private float facingAngle;
    private PolygonCollider2D pc;
    [SerializeField] private Vector3 currentPos;
    #endregion

    #region action
    [SerializeField] private GameObject activator;
    #endregion
    void Start()
    {
        isTracking = false;
        detectedObject = null;
        pc = GetComponent<PolygonCollider2D>();
        currentPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    private void Update()
    {
        ColliderConstruction();
        TrackPlayer(detectedCollider);
    }

    private void TrackPlayer(Collider2D other)
    {
        //if there's something to track..
        if (detectedCollider != null)
        {
            //create an array of everything that the ray WILL hit
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(detectCone.transform.position, (other.transform.position - detectCone.transform.position).normalized);
            //fill the array with a raycast all, but only look at first value. This is so that anything in the way of the player will block alarms
            if (hits != null && hits.Length > 0)
            {
                GameObject hit = hits[0].collider.gameObject;
                what = hit;
                //Debug.Log("hitting " + hit);
                Debug.DrawLine(detectCone.transform.position, other.transform.position, Color.blue);
                float rayDistance = Vector2.Distance(detectCone.transform.position, hit.transform.position);
                //Debug.Log(Vector2.Distance(detectCone.transform.position, hit.transform.position));
                //Debug.Log(Vector2.SignedAngle(Vector2.down, hit.transform.position));
                if (hit.CompareTag("Player") && rayDistance < detectDistance)
                {
                    detectCone.color = Color.red;
                    activator.GetComponent<Animator>().SetBool("Activate", true);
                }
                else
                {
                    activator.GetComponent<Animator>().SetBool("Activate", false);
                    detectCone.color = Color.green;
                }
            }
            //if theere's nothing to hit, not tracking anything
            if (hits == null || hits.Length == 0)
            {
                what = null;
                activator.GetComponent<Animator>().SetBool("Activate", false);
                detectCone.color = Color.green;
                return;
            }
        }
        else
        {
            activator.GetComponent<Animator>().SetBool("Activate", false);
            detectCone.color = Color.green;
            return;
        }
    }
    private void ColliderConstruction()
    {
        cPos = detectCone.transform.position;
        currentPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        detectConeAngle = detectCone.pointLightOuterAngle;
        detectDistance = detectCone.pointLightOuterRadius;
        //sets the origin and center point
        Vector2 origin = new Vector2(0,0);
        Vector2 centerPoint = new Vector2(0, detectDistance);
        //creates a vector array to build the polygon colilder
        Vector2[] vertices = new Vector2[]
        {
            new Vector2(0, 0), // origin
            new Vector2(1, 0), // right
            new Vector2(0, 1), // left
        };
        //sets the angles that it will need to move towards to create the new vertices
        float totalAngle = detectConeAngle;
        float halfAngle = totalAngle * .5f;
        float compAngle = 90f - halfAngle;
        float halfRad = halfAngle * Mathf.Deg2Rad;
        //find the length of the right side's line through finding its vector
        float rightHypotenuse = detectDistance/Mathf.Cos(halfRad);
        //find the height of the right half of the polygon triangle
        float rightOpposite = rightHypotenuse*Mathf.Sin(halfRad);
        Vector2 rightVert = new Vector2(detectDistance, rightOpposite);
        Vector2 leftVert = new Vector2(detectDistance, -rightOpposite);
        vertices[1] = rightVert;
        vertices[2] = leftVert;
        //creates the vertices points
        pc.points = vertices;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        isTracking = true;
        if (other.gameObject.CompareTag("Untagged"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Player")) //change this to track more types of objects, you can add tags
        {
            detectedObject = other.gameObject;
            detectedCollider = other;
        }
        else
        {
            return;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        //"turns off" the tracker so it's not always firing rays
        if (other.gameObject.CompareTag("Player"))
        {
            detectedCollider = null;
            isTracking = false;
            detectedObject = null;
        }
        else
        {
            return;
        }
    }
}
