using System;
using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.FloatEvent;
using UniRx;
using UnityEngine;

[CreateAssetMenu]
public class PlayerInfo : ScriptableObject
{
    [Header("Dependency Injection")]
    [SerializeField] private PercentageFloatReactivePropertySO _virusSO;

    // any information about the player we want tracked can
    // be stored here.
    [Header("Info")]
    public float battery;
    public float maxBattery;
    [HideInInspector] public IReactiveProperty<float> virus;
    public float maxVirus;
    public float iframesTime;

    private void OnValidate()
    {
        virus = _virusSO;
    }
}
