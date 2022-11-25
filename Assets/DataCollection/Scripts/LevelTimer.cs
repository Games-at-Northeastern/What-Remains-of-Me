using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private DatabaseFloat _levelTime;

    private void Update()
    {
        if (Time.timeScale != 0.0f)
        {
            _levelTime.IncrementByDeltaTime();
        }
    }
}
