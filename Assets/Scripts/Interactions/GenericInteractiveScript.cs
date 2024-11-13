using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class GenericInteractiveScript : MonoBehaviour
{
    public List<string> tagsToInteractWith = new List<string> { "Player", "Interactable" };
    private Interaction interaction;
    protected bool objectInRange = false;


    protected ControlSchemes _cs;


    void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        Debug.Log("starting to interact");




        interaction = GetComponent<Interaction>();
        _cs = new ControlSchemes();
        _cs.Enable();
    }


    // Update is called once per frame
    void Update()
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


    protected void Interact()
    {
        interaction.Execute();
    }


}

