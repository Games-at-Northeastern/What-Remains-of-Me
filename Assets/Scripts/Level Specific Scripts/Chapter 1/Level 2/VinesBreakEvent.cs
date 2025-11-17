using Ink.Parsed;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
///
/// </summary>
public sealed class VinesBreakEvent : MonoBehaviour, IOnCollision
{
    [SerializeField]
    private UnityEvent OnEventOccur;
    [SerializeField]
    private UnityEvent<bool> ToDisableUponInvoke;

    [SerializeField]
    private List<GameObject> objs = new List<GameObject>();


    [SerializeField]
    private bool _oneShot = true;

    public void Collide(Vector2 direction)
    {
        OnEventOccur.Invoke();

        ToDisableUponInvoke.Invoke(false);

        if (_oneShot)
        {
            foreach (GameObject obj in objs)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
