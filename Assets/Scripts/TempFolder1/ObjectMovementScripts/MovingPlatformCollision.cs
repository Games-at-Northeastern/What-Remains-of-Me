using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollision : MonoBehaviour
{
    public List<Rigidbody2D> objectsOnPlatform;
    [SerializeField] TestMovingObjectScript movingObjectScript;
    private void Start()
    {
        objectsOnPlatform = new List<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            objectsOnPlatform.Add(rb);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            objectsOnPlatform.Remove(rb);
        }
    }
}
