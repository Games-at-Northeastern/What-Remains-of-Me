using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an object which can be controlled via an outlet.
/// </summary>
public interface IControllable
{
    /// <summary>
    /// Supplies this object with the given amount of energy. This can have
    /// a large number of effects, depending on what the object is.
    /// </summary>
    public void GainEnergy(float amount);

    /// <summary>
    /// Drains the given amount of energy from this object. This can have
    /// a large number of effects, depending on what the object is.
    /// </summary>
    public void LoseEnergy(float amount);
}
