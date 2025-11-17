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

    public override void Connect()
    {
        base.Connect();

        if (grantOnce && hasKey)
            return;
        if (grantRoutine != null)
            return;

        grantRoutine = StartCoroutine(GetKey());
        Debug.Log("Start getting key! ");
    }

    public override void Disconnect()
    {
        base.Disconnect();
        grantRoutine = null;
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


