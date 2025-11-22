using Ink.Parsed;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
///
/// </summary>
public sealed class VinesBreakEvent : MonoBehaviour, IOnCollision
{
    [SerializeField]
    private UnityEvent OnEventOccur;
    [SerializeField]
    private UnityEvent<bool> ToDisableUponInvoke;

    [Tooltip("These objects will be deleted")]
    [SerializeField] private List<GameObject> vineAnchors = new List<GameObject>();

    [Tooltip("A random horizontal force will applied to these")]
    [SerializeField] private List<GameObject> bonesToApplyForceTo = new List<GameObject>();

    [Tooltip("The max the horizontal force that can be applied")]
    [SerializeField] private float maxForce = 20f;


    [SerializeField]
    private bool _oneShot = true;

    [Tooltip("FOR DEBUGGING WILL CAUSE THE TRIGGER EFFECT TO HAPPEN")]
    [SerializeField] private bool testTrigger = false;

    public void Update()
    {
        // This is here for Debugging purposes so you can easily test the trigger effect without going
        // through all the steps of conducting it
        if (testTrigger)
        {
           Collide(Vector2.zero);
        }
    }

    public void Collide(Vector2 direction)
    {
        OnEventOccur.Invoke();

        ToDisableUponInvoke.Invoke(false);

        if (_oneShot)
        {
            foreach (GameObject obj in vineAnchors)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            foreach (GameObject obj in bonesToApplyForceTo)
            {
                var rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(new Vector2(Random.Range(-maxForce, maxForce), 0), ForceMode2D.Impulse);
                }
            }
            _oneShot = false;
        }
    }
}
