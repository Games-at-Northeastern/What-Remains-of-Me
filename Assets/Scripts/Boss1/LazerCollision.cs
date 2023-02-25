using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerCollision : MonoBehaviour
{

    public PlayerInfo player;
    public int energyAmount;


     void OnTriggerStay2D (Collider2D other)
     {
         if (other.gameObject.tag == "Player")
         {
            player.battery += energyAmount * Time.fixedDeltaTime;
     }
     }
}