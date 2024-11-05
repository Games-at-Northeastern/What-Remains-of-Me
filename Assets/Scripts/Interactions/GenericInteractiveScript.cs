using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GenericInteractiveScript : MonoBehaviour
{

    private bool playerInRange = false;

    public bool interactOnKeyPress = false;
    public bool interactOnCollision = false;
    private ControlSchemes _cs;
    private BoxCollider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting to interact");
        _collider = GetComponent<BoxCollider2D>();
        _cs = new ControlSchemes();
        _cs.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        if (interactOnCollision)
        {
            if (playerInRange)
            {
                Interact();
                playerInRange = false;
            }
        }

        else if (interactOnKeyPress)
        {
            if (playerInRange && _cs.Player.Dialogue.WasPressedThisFrame())
            {
                Debug.Log("Interacted with F");
                //Interact();
            }
        }
    }

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

    private void Interact()
    {
        Debug.Log("We are interacting rn.");
    }

}
