using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnKeyPress : GenericInteractiveScript
{
    protected override void AttemptInteract()
    {
        playerInRange = false;
    }
}
