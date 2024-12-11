using System.Collections.Generic;
using UnityEngine;

public abstract class GenericInteractiveScript : MonoBehaviour
{
    public List<string> tagsToInteractWith = new() { "Player", "Interactable" };
    private Interaction interaction;
    protected bool objectInRange = false;

    protected ControlSchemes cs;

    private void Start() => Init();
    protected virtual void Init()
    {
        Debug.Log("starting to interact");

        interaction = GetComponent<Interaction>();
        cs = new ControlSchemes();
        cs.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        if (objectInRange)
        {
            AttemptInteract();
        }
    }

    protected abstract void AttemptInteract();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tagsToInteractWith.Contains(other.gameObject.tag))
        {
            objectInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (tagsToInteractWith.Contains(other.gameObject.tag))
        {
            objectInRange = false;
        }
    }

    protected void Interact() => interaction.Execute();
}
