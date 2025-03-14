using UnityEngine;

public class MovingDown : MonoBehaviour
{
    public float speed = 2f; // Speed at which the object moves down

    void Update()
    {
        // Move the object downward at a constant speed
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}
