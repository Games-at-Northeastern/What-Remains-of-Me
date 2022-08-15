using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI view of the player's health.
/// </summary>
public class PlayerHealthView : MonoBehaviour
{
    [SerializeField] Slider slider;

    void Start()
    {
        PlayerHealth.instance.OnHealthChanged.AddListener(() => UpdateHealthView());
        UpdateHealthView();
    }

    void UpdateHealthView()
    {
        slider.value = PlayerHealth.GetRelativeBattery() * slider.maxValue;
    }
}
