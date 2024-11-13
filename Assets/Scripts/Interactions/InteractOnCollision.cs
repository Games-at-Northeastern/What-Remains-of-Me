using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnCollision : GenericInteractiveScript
{
    protected override void AttemptInteract()
    {
        Interact();
        objectInRange = false;
    }
}
