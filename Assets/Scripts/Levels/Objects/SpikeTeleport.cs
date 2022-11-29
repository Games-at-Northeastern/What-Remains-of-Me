using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTeleport : MonoBehaviour
{

    [SerializeField] Transform teleportLocation;


    void OnTriggerEnter2D (Collider2D other)
     {
         if (other.gameObject.tag == "Player")
         {
            other.transform.position = teleportLocation.position;
         }
     }
}
