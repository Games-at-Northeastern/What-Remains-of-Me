using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A collision event for a platform in which it breaks a pane of glass.
/// </summary>
public sealed class ShatterEvent : MonoBehaviour, IOnCollision
{
    [SerializeField]
    private UnityEvent OnEventOccur;
    [SerializeField]
    private UnityEvent<bool> ToDisableUponInvoke;


    [SerializeField]
    private bool _oneShot = true;

    public void Collide(Vector2 direction)
    {
        OnEventOccur.Invoke();

        ToDisableUponInvoke.Invoke(false);

        if (_oneShot)
        {
            Destroy(this);
        }
    }
}
