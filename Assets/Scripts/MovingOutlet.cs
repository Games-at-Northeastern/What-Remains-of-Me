using UnityEngine;

public class MovingOutlet : MonoBehaviour
{
    [SerializeField] private Transform outletTransform; //transform of the outlet that will be moving
    [SerializeField] private Transform pointA; // Start point
    [SerializeField] private Transform pointB; // End point
    [SerializeField] private float speed = 2f; // Movement speed
    
    private Vector2 targetPosition; // Target position

    private void Start()
    {
        targetPosition = pointB.position; // Set initial target position to point B
    }

    private void Update()
    {
        // Move towards the target position
        outletTransform.position = Vector2.MoveTowards(outletTransform.position, targetPosition, speed * Time.deltaTime);

        // Check if the platform has reached the target position
        if ((Vector2)outletTransform.position == targetPosition)
        {
            // Switch the target position between point A and point B
            if (targetPosition == (Vector2)pointA.position)
            {
                targetPosition = (Vector2)pointB.position;
            }
            else
            {
                targetPosition = (Vector2)pointA.position;
            }
        }
    }
}
