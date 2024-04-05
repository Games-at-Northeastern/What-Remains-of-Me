using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An abstract class for playing effects in real time
public abstract class Effects : MonoBehaviour
{
    //Function to call to play the desired effects from child classes
    public abstract void PlayEffect();

    //Function to call to stop the desired effects from child classes
    public abstract void CancelEffect();
}
