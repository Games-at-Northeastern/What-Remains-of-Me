using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives constants which allow wire movement to be customized from the editor. The
/// settings in this script should be accessible to the wire movement scripts, so that
/// they can be used.
/// Note: Serialized fields and accessible fields are kept separate so that
/// the information here can be customized from the editor, but not changed
/// from any other script.
/// </summary>
public class PlugMovementSettings : MonoBehaviour
{
    // SERIALIZED FIELDS

    [Header("Straight")]
    [SerializeField] float straightSpeed;
    [SerializeField] float straightTimeTillRetraction;

    // ACCESSIBLE FIELDS

    // Straight
    public float StraightSpeed { get { return straightSpeed; } }
    public float StraightTimeTillRetraction { get { return straightTimeTillRetraction; } }
}
