using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private bool TimeStopped = true;
    private void Update()
    {
        if (TimeStopped)
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
            }
        }
    }
    public void StartTime()
    {
        TimeStopped = false;
        Time.timeScale = 1.0f;
    }
    public void StopTime()
    {
        Time.timeScale = 0.0f;
    }
}
