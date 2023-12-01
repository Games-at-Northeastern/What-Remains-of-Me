using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides class instances which are important to move scripts.
/// Note: Serialized fields and accessible fields are kept separate so that
/// the information here can be customized from the editor, but not changed
/// from any other script.
/// </summary>
public class MovementInfo : MonoBehaviour
{
    [SerializeField] private CollisionDetector damageDetector;

    public CollisionDetector DamageDetector => damageDetector;
}
