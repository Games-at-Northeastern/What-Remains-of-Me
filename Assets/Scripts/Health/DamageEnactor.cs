using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Figures out when the player is hitting damaging material, and uses
/// that to try and make the player take damage.
/// 
/// ~~~CURRENTLY NOT IN USE~~~
/// </summary>
public class DamageEnactor : MonoBehaviour
{
    [SerializeField] MovementInfo MI;
    [SerializeField] PlayerHealth PH;
    [SerializeField] float damageAmount;
    [SerializeField] Animator a;

    /// <summary>
    /// checks if this gameObject is colliding with any other colliders
    /// and calls for damage to be take through the PlayerHealth component.
    /// also calls the PlayerDamaged animation
    /// </summary>
    private void Update()
    {
        if (MI.DamageDetector.isColliding())
        {
            PH.RequestTakeDamage(damageAmount);
            a.Play("PlayerDamaged");
        }
    }
}
