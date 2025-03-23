using UnityEngine;

public class SecurityCameraFollow : MonoBehaviour
{

    SpriteRenderer renderer;

    public Sprite camFarSight;
    public Sprite camMidSight;
    public Sprite camNearSight;
    public Sprite camUnderSight;

    public bool inRange = false;

    // When the camera determines player is in range to track.
    public float detectionRadius = 15;

    GameObject player;

    public Vector2 direction;

    // Note that this class is for a camera prop in game and not the actual scene camera.
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!renderer)
        {
            return;
        }

        direction = player.transform.position - transform.position;

        if (Mathf.Abs(direction.x) < detectionRadius)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        if (inRange)
        {

            if (direction.x < 0)
            {
                renderer.flipX = true;
                UpdateSprite(Mathf.Abs(direction.x));
            }
            else if (direction.x > 0)
            {
                renderer.flipX = false;
                UpdateSprite(Mathf.Abs(direction.x));
            }


        }
    }

    // Values of X of how close the player is to the camera.
    void UpdateSprite(float distanceX)
    {
        if (distanceX < 0.5f)
        {
            renderer.sprite = camUnderSight;
        }
        else if (distanceX < 1f)
        {
            renderer.sprite = camNearSight;
        }
        else if (distanceX < 2f)
        {
            renderer.sprite = camMidSight;
        }
        else
        {
            renderer.sprite = camFarSight;
        }

    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


}
