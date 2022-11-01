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
    // Serialized Fields
    [SerializeField] private CollisionDetector groundDetector;
    [SerializeField] private CollisionDetector ceilingDetector;
    [SerializeField] private CollisionDetector leftWallDetector;
    [SerializeField] private CollisionDetector rightWallDetector;
    [SerializeField] private CollisionDetector damageDetector;

    // Accessible Fields
    public CollisionDetector GroundDetector => groundDetector;
    public CollisionDetector CeilingDetector => ceilingDetector;
    public CollisionDetector LeftWallDetector => leftWallDetector;
    public CollisionDetector RightWallDetector => rightWallDetector;
    public CollisionDetector DamageDetector => damageDetector;
}
