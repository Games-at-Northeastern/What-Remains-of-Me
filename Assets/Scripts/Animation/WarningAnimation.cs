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
    [SerializeField] private GameObject lightA;
    [SerializeField] private GameObject lightB;
    private float time;
    private Animator anim;
    private Color colorA;
    private Color colorB;

    /// <summary>
    /// Initializes animation to not run, and initializes time
    /// </summary>
    void Start()
    {
        _runAnimation = false;
        time = -1.0f;
        anim = warning.GetComponent<Animator>();
    }

    /// <summary>
    /// If the animation should running, updates animation so that the warning turns on and off every second 
    /// </summary>
    void Update()
    {
        if (anim == null && _runAnimation)
        {
            if (time <= 0.1)   // Since time is set to a negative number in Start and StopAnimation, 
                                // this line is always true on first update when _runAnimation is true
            {
                //warning.GetComponent<UnityEngine.UI.Image>().enabled = !warning.GetComponent<UnityEngine.UI.Image>().isActiveAndEnabled;
                warning.GetComponent<SpriteRenderer>().enabled = !warning.GetComponent<SpriteRenderer>().enabled;
                time = 0.1f;
            }
            else if (time >= 1.1)
            {
                time = 0.0f;
            }
            time += Time.deltaTime;
        }
    }

    /// <summary>
    /// Starts the animation
    /// </summary>
    public void StartAnimation()
    {
        _runAnimation = true;
        if (anim != null)
        {
            anim.enabled = true;
            warning.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    /// <summary>
    /// Ends the animation. Turns off the warning image and resets the time
    /// </summary>
    public void StopAnimation()
    {
        _runAnimation = false;
        warning.GetComponent<SpriteRenderer>().enabled = false;
        time = -1.0f;
        if (anim != null)
        {
            anim.enabled = false;
            warning.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void HighVirus()
    {

    }

    public void RegularVirus()
    {

    }
}
