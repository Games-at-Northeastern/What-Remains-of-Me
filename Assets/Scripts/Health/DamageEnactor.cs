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

    private void Awake()
    {
        MI = GetComponent<MovementInfo>();
        PH = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (MI.DamageDetector.isColliding())
        {
            PH.RequestTakeDamage(damageAmount);
        }
    }
}
