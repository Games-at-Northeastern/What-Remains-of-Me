using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartScriptableObjects.ReactiveProperties;

public class SetPlayerEnergyLevelOnStart : MonoBehaviour
{

    [SerializeField] private PercentageFloatReactivePropertySO playerBatterySO;

    [Header("How much energy the player should start with between 0 and 1")]
    [SerializeField] private float PlayerEnergyStartingValue = 1f;

    void Start()
    {
        playerBatterySO.Value = PlayerEnergyStartingValue;
        
    }



}
