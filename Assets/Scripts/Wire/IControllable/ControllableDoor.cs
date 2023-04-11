using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a door which can be moved up or down based on the amount of energy
/// supplied to it.
/// </summary>
public class ControllableDoor : AControllable
{
    Vector2 initPos;
    [Header("Position In Editor: Zero Energy")]
    [SerializeField] Vector2 posChangeForMaxEnergy;

    private void Awake()
    {
        initPos = transform.position;
    }

    /// <summary>
    /// Updates the door's position based on the amount of energy supplied to it.
    /// </summary>
    void Update()
    {
        transform.position = Vector2.Lerp(initPos, initPos + posChangeForMaxEnergy, (energy + virus) / maxCharge);
    }
}
