using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartScriptableObjects.ReactiveProperties;

public class SetPlayerEnergyLevelOnStart : MonoBehaviour
{

    [SerializeField] private PercentageFloatReactivePropertySO playerBatterySO;
    [SerializeField] private PercentageFloatReactivePropertySO playerVirusSO;


    [Header("How much energy the player should start with between 0 and 1")]
    [SerializeField] private float PlayerEnergyStartingValue = 1f;

    [Header("How much virus the player should start with between 0 and 1")]
    [SerializeField] private float PlayerVirusStartingValue = 0f;

    void Start()
    {
        playerBatterySO.Value = PlayerEnergyStartingValue;
        playerVirusSO.Value = PlayerVirusStartingValue;
        
    }



}
