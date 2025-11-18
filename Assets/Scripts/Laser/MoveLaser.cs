using UnityEngine;

public class MoveLaser : MonoBehaviour
{
    public float speed = 5f;
    public float timeMoving = 2f;
    public float timer = 0f;
    public bool movingLeft = true;

    void Update()
    {
        if (movingLeft)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        
        timer += Time.deltaTime;
        
        if (timer >= timeMoving)
        {
            movingLeft = !movingLeft;
            timer = 0f;
        }
    }
}