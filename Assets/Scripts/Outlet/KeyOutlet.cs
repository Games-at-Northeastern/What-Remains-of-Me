using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyOutlet : Outlet
{
    [SerializeField] private bool grantOnce = true;
    [SerializeField] private float delayBeforeGrant = 2f;

    private List<IAlarmListener> listeners = new List<IAlarmListener>();

    public static bool hasKey = false;
    private Coroutine grantRoutine;

    private void Awake()
    {
        CS = new ControlSchemes();
        CS.Player.TakeEnergy.performed += _ => { if (!(grantOnce && hasKey)) { grantRoutine = StartCoroutine(GetKey()); } };
        CS.Player.TakeEnergy.canceled += _ => { StopAllCoroutines(); grantRoutine = null; };
    }
    
    float timer = 0f;

    private IEnumerator GetKey()
    {
        while (timer < delayBeforeGrant)
        {
            Debug.Log(timer);
            timer += Time.deltaTime;
            yield return null;
        }

        hasKey = true;
        Debug.Log("Player has key! ");

        foreach (IAlarmListener listener in listeners)
        {
            listener.OnAlarmStart();
        }

        grantRoutine = null;
    }

    public void Subscribe(IAlarmListener listener)
    {
        listeners.Add(listener);
    }
}


