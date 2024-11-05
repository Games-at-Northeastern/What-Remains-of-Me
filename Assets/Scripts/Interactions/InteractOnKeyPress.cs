using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnKeyPress : GenericInteractiveScript
{
<<<<<<< Updated upstream
    protected override void AttemptInteract()
    {
        playerInRange = false;
=======

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

>>>>>>> Stashed changes
    }

    protected override void AttemptInteract()
    {
        if (base._cs.Player.Dialogue.WasPressedThisFrame())
        {
            Interact();
        }

    }

}
