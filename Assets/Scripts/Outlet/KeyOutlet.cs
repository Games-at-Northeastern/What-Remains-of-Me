using UnityEngine;

public class KeyGrantOutlet : Outlet
{
    [SerializeField] private AutoOpenDoor door;
    [SerializeField] private bool grantOnce = true;

    private bool granted = false;

    public override void Connect()
    {
        base.Connect();

        if (grantOnce && granted)
            return;
        if (!door)
            return;

        door.SetHasKey();
        granted = true;

        Debug.Log("Player has key! ");
    }

    public override void Disconnect()
    {
        base.Disconnect();
    }
}


