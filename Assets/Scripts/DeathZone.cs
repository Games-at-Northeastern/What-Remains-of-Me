using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DeathZone : MonoBehaviour
{
    //Should this death zone instantly kill the player, or just deal damage 
    public bool instakill = true;
    //If this should deal damage to the batter instead, how much 
    public float batteryDamage;
    //Collider and death zone for this class 
    private BoxCollider2D collider;
    //Player health to use in internal functions 
    private PlayerHealth playerHealthScript;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        playerHealthScript = GameObject.Find("Player").GetComponentInChildren(typeof(PlayerHealth)) as PlayerHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (instakill)
            {
                playerHealthScript.LoseEnergy(playerHealthScript.playerInfo.maxBattery);
            } else
            {
                playerHealthScript.LoseEnergy(batteryDamage);
            }
        }
    }
}
