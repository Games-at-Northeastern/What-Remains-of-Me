public class InteractOnKeyPress : GenericInteractiveScript
{
    protected override void AttemptInteract()
    {

        if (cs.Player.Dialogue.WasPressedThisFrame())
        {
            Interact();
        }
    }
}
