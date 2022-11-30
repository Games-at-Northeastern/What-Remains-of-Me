using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Button script to be used with doors in game. Currently three types of puzzle/door interaction.
//Specify this type using the 'PuzzleButtonType' field.
//
//  - toggleOnPermanent - Pressing the button will permanantly open the door.
//  - holdOpen - Pressing the button will open the door as long as the button is pressed. As soon as the player leaves,
//               the door will immediately close.
//  - toggleOnTimer - Pressing the button will hold the door open for the specified 'holdOpenTime' field. 
[RequireComponent(typeof(BoxCollider2D))]
public class PuzzleButton : MonoBehaviour
{
    private BoxCollider2D collider; //Collider used to detect player

    public PuzzleButtonType type; //Type of button/door interaction
    public AbstractPuzzleDoor thisDoor; //Door object to be opened/closed based on button
    public float holdOpenTime; //If this is a button that holds a door for a set ammount of time, specify it here

    private bool playerTouching;

    private float time;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        playerTouching = false;
        time = 0;

        if (thisDoor == null)
        {
            Debug.LogWarning("No door object assigned to " + gameObject.name);
        }
    }

    void FixedUpdate()
    {
        if (type == PuzzleButtonType.toggleOnTimer)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            } else
            {
                thisDoor.CloseDoor();
            }
        }
    }

    //Handle proper door behavior based on button type 
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && playerTouching == false)
        {
            playerTouching = true;
            switch (type)
            {
                case PuzzleButtonType.holdOpen:
                case PuzzleButtonType.toggleOnPermanent:
                    thisDoor.OpenDoor();
                    break;
                case PuzzleButtonType.toggleOnTimer:
                    thisDoor.OpenDoor();
                    time = holdOpenTime;
                    break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerTouching = false;
            if (type == PuzzleButtonType.holdOpen)
            {
                thisDoor.CloseDoor();
            }
        }
    }
}

public enum PuzzleButtonType
{
    toggleOnPermanent,
    holdOpen,
    toggleOnTimer
}


