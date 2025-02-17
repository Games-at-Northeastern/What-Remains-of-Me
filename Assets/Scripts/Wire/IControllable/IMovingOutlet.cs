using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// A stopgap implementation of an interface to ensure that both movingelement and movingoutlet outlets can properly transfer
/// their velocity to the player for swing detection
/// TODO: Replace moving outlets with moving elements. Once that is done, this interface will be made redudant and can be properly put to death.
/// </summary>
public interface IMovingOutlet
{
    public Vector3 MovementVector();
}
