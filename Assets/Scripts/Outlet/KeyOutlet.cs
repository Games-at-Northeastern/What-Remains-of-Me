using UnityEngine;
using System.Collections;

public class KeyGrantOutlet : Outlet
{
    [SerializeField] private AutoOpenDoor door;
    [SerializeField] private bool grantOnce = true;
    [SerializeField] private float delayBeforeGrant = 2f;

    private bool granted = false;
    private Coroutine grantRoutine;

    public override void Connect()
    {
        base.Connect();

        if (grantOnce && granted)
            return;
        if (!door)
            return;
        if (grantRoutine != null)
            StopCoroutine(grantRoutine);

        grantRoutine = StartCoroutine(GetKey());
    }

    public override void Disconnect()
    {
        base.Disconnect();
        grantRoutine = null;
    }

    private IEnumerator GetKey()
    {
        var timer = 0f;

        while (timer < delayBeforeGrant)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        door.SetHasKey();
        granted = true;
        Debug.Log("Player has key! ");

        grantRoutine = null;
    }
}


