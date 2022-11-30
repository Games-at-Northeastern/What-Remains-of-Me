using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An abstract class represnting a door to be used in puzzles (in tandem with PuzzleButton.cs)
//Doesn't necessarily need to be a door physically, provides an Open and Clos behavior (could be a wall, window, etc.) 
public abstract class AbstractPuzzleDoor : MonoBehaviour
{
    public bool isOpen;
    public abstract void OpenDoor();

    public abstract void CloseDoor();
}
