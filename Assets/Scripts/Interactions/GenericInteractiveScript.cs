using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GenericInteractiveScript : MonoBehaviour
{

    private Interaction interaction;
    protected bool playerInRange = false;

    protected ControlSchemes _cs;
    private BoxCollider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        Debug.Log("starting to interact");
        interaction = GetComponent<Interaction>();
        _collider = GetComponent<BoxCollider2D>();
        _cs = new ControlSchemes();
        _cs.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            AttemptInteract();
        }
    }

    protected abstract void AttemptInteract();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    protected void Interact()
    {
        interaction.Execute();
        Debug.Log("We are interacting rn.");
    }

}
