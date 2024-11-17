using UnityEngine;

public class MoveLeftRight : MonoBehaviour
{
    public float speed; // Movement speed
    public float distance; // Distance to move left and right

    private Vector3 startPos;

    void Start()
    {
        // Save the starting position
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate new position using sine wave for smooth left-right motion
        float move = Mathf.Sin(Time.time * speed) * distance;

        // Move the object left and right
        transform.position = startPos + new Vector3(move, 0, 0);
    }
}