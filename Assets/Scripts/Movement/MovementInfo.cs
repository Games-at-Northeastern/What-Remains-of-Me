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
    [SerializeField] CollisionDetector groundDetector;
    [SerializeField] CollisionDetector ceilingDetector;
    [SerializeField] CollisionDetector leftWallDetector;
    [SerializeField] CollisionDetector rightWallDetector;
    [SerializeField] CollisionDetector damageDetector;

    // Accessible Fields
    public CollisionDetector GroundDetector { get { return groundDetector; } }
    public CollisionDetector CeilingDetector { get { return ceilingDetector; } }
    public CollisionDetector LeftWallDetector { get { return leftWallDetector; } }
    public CollisionDetector RightWallDetector { get { return rightWallDetector; } }
    public CollisionDetector DamageDetector { get { return damageDetector; } }
}
