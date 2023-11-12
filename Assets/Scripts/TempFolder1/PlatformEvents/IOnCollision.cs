using UnityEngine;

/// <summary>
/// An interface for things that platforms collide into. Implement this for events that occur
/// when a platform smashes into them, like glass breaking.
/// </summary>
public interface IOnCollision
{
    void Collide(Vector2 direction);
}
