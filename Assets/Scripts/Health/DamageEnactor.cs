using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Figures out when the player is hitting damaging material, and uses
/// that to try and make the player take damage.
/// </summary>
[RequireComponent(typeof(MovementInfo))]
[RequireComponent(typeof(PlayerHealth))]
public class DamageEnactor : MonoBehaviour
{
    MovementInfo MI;
    PlayerHealth PH;
    [SerializeField] float damageAmount;

    /*
     * gets the the script components for MovementInfo and PlayerHealth
     * from this gameObject.
     * 
     */
    private void Awake()
    {
        MI = GetComponent<MovementInfo>();
        PH = GetComponent<PlayerHealth>();
    }

    /*
     * checks if this gameObject is colliding with any other colliders
     * and calls for damage to be take through the PlayerHealth component
     * 
     */
    private void Update()
    {
        if (MI.DamageDetector.isColliding())
        {
            PH.RequestTakeDamage(damageAmount);
        }
    }
}
