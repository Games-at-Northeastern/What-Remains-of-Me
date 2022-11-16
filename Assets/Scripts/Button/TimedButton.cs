using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A button that can be used to activate an AObjectToActivate. Activate for a set amount of time, then deactivate
/// </summary>
public class TimedButton : Button
{
    public float ActivateTime; // The duration of this button's activation

    private float CountDown; // Timer used to track time

    /// <summary>
    /// Decrement CountDown in each update to record time. When countdown is over, deactivate the button
    /// </summary>
    void Update()
    {
        if (CountDown > 0)
        {
            CountDown -= Time.deltaTime;
            if (CountDown <= 0)
            {
                OTA.Deactivate();
            }
        }
    }

    /// <summary>
    /// Reset the countdown and activate the object when pressed
    /// </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!CheckTag || other.gameObject.tag == TagOfOther)
        {
            CountDown = 0;
            OTA.Activate();
        }
    }

    /// <summary>
    /// Start the timer when no longer pressed
    /// </summary>
    void OnCollisionExit2D(Collision2D other)
    {
        if (!CheckTag || other.gameObject.tag == TagOfOther)
        {
            CountDown = ActivateTime;
        }
    }
}
