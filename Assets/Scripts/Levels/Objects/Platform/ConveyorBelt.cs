using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Tooltip("How fast the conveyor moves. negative values are counter clockwise. positive are clockwise")]
    public float speed;

    /// <summary>
    /// Returns the force applied along the surface normal of the conveyor belt
    /// </summary>
    /// <returns></returns>
    private Vector2 ConveyorVelocity()
    {
        return new Vector2(Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z), Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z)) * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            collision.rigidbody.velocity += ConveyorVelocity() * Time.fixedDeltaTime;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            collision.attachedRigidbody.AddForce(ConveyorVelocity());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.AddForce(ConveyorVelocity(), ForceMode2D.Impulse);
        }
    }
}
