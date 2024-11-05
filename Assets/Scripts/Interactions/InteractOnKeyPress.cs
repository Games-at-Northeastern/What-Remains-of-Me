using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnKeyPress : GenericInteractiveScript
{
    protected override void AttemptInteract()
    {
        if (_cs.Player.Dialogue.WasPressedThisFrame())
        {
            Interact();
        }

    }

}
