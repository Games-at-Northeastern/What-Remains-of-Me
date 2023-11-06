using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates a warning for when a player is about to die. Currently, causes warning image to blink
/// </summary>
public class WarningAnimation : MonoBehaviour
{
    private bool _runAnimation;
    [SerializeField] private GameObject warning;
    private float time;

    /// <summary>
    /// Initializes animation to not run, and initializes time
    /// </summary>
    void Start()
    {
        _runAnimation = false;
        time = 0.0f;
    }

    /// <summary>
    /// If the animation should running, updates animation so that the warning turns on and off every second 
    /// </summary>
    void Update()
    {
        if (_runAnimation)
        {
            if (time <= 0.1)   // Since time is set to a negative number in Start and StopAnimation, 
                                // this line is always true on first update when _runAnimation is true
            {
                warning.GetComponent<UnityEngine.UI.Image>().enabled = !warning.GetComponent<UnityEngine.UI.Image>().isActiveAndEnabled;
                time = 0.11f;
            }
            else if (time >= 1.1)
            {
                time = 0.0f;
            }
            time += Time.deltaTime;
           // Debug.Log(time);
        }
    }

    /// <summary>
    /// Starts the animation
    /// </summary>
    public void StartAnimation()
    {
        _runAnimation = true;
    }

    /// <summary>
    /// Ends the animation. Turns off the warning image and resets the time
    /// </summary>
    public void StopAnimation()
    {
        _runAnimation = false;
        warning.GetComponent<UnityEngine.UI.Image>().enabled = false;
        time = 0.0f;
    }
}
