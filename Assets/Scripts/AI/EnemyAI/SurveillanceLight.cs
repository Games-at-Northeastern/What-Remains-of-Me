using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceLight : MonoBehaviour
{
    private bool isColliding;

    public bool GetCollisionStatus()
    {
        return isColliding;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isColliding = false;
        }
    }
}
